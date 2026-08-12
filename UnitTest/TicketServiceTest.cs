using Application.Services;
using Moq;
using Technical_Assessment_ElectroPi.Application.Services;
using Technical_Assessment_ElectroPi.Contract;
using Technical_Assessment_ElectroPi.Core.Entities;
using Technical_Assessment_ElectroPi.Core.Entities.Enums;

namespace UnitTest
{
    public class TicketServiceTest
    {
        private readonly Mock<ITicketRepository> _mockTicketRepository;
        private readonly Mock<ITicketActivityRepository> _mockTicketActivityRepository;
        private readonly Mock<ITicketCommentRepository> _mockTicketCommentRepository;
        private readonly Mock<ITimeEntryRepository> _mockTimeEntryRepository;
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<ICurrentUser> _mockCurrentUser;

        private readonly ITicketService _ticketService;

        public TicketServiceTest()
        {
            _mockTicketRepository =
                new Mock<ITicketRepository>();

            _mockTicketActivityRepository =
                new Mock<ITicketActivityRepository>();

            _mockTicketCommentRepository =
                new Mock<ITicketCommentRepository>();

            _mockTimeEntryRepository =
                new Mock<ITimeEntryRepository>();

            _mockUnitOfWork =
                new Mock<IUnitOfWork>();

            _mockCurrentUser =
                new Mock<ICurrentUser>();

            _ticketService = new TicketService(
                _mockTicketRepository.Object,
                _mockTicketActivityRepository.Object,
                _mockTicketCommentRepository.Object,
                _mockTimeEntryRepository.Object,
                _mockUnitOfWork.Object,
                _mockCurrentUser.Object
            );
        }
        [Fact]
        public async Task GetTicketByIdAsync_ShouldReturnTicket_WhenTicketExists()
        {
            // Arrange
            var customerId = Guid.NewGuid();

            var ticket = Ticket.Create(
                "TKT-001",
                "Login issue",
                "Customer cannot login",
                customerId,
                TicketPriority.Medium);

            _mockTicketRepository
                .Setup(repo => repo.GetByIdAsync(
                    ticket.Id,
                    CancellationToken.None))
                .ReturnsAsync(ticket);

            // Act
            var result = await _ticketService.GetByIdAsync(ticket.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(ticket.Id, result.Id);
            Assert.Equal("TKT-001", result.TicketNumber);
            Assert.Equal("Login issue", result.Title);
            Assert.Equal(TicketStatus.Open, result.Status);

            _mockTicketRepository.Verify(
                repo => repo.GetByIdAsync(
                    ticket.Id,
                    CancellationToken.None),
                Times.Once);
        }
        [Fact]
        public async Task AssignAgentAsync_ShouldAssignAgentAndSaveChanges()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var agentId = Guid.NewGuid();
            var performedByUserId = Guid.NewGuid();

            var ticket = Ticket.Create(
                "TKT-001",
                "Login issue",
                "Customer cannot login",
                customerId,
                TicketPriority.Medium);

            var ticketId = ticket.Id;

            _mockTicketRepository
                .Setup(repo => repo.GetByIdAsync(
                    ticketId,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(ticket);

            _mockTicketActivityRepository
                .Setup(repo => repo.AddAsync(
                    It.IsAny<TicketActivity>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _mockUnitOfWork
                .Setup(unit => unit.SaveChangesAsync(
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(1);

            // Act
            await _ticketService.AssignAgentAsync(
                ticketId,
                agentId,
                performedByUserId,
                CancellationToken.None);

            // Assert
            Assert.Equal(
                agentId,
                ticket.AssignedAgentId);

            _mockTicketActivityRepository.Verify(
                repo => repo.AddAsync(
                    It.Is<TicketActivity>(activity =>
                        activity.TicketId == ticket.Id),
                    It.IsAny<CancellationToken>()),
                Times.Once);

            _mockUnitOfWork.Verify(
                unit => unit.SaveChangesAsync(
                    It.IsAny<CancellationToken>()),
                Times.Once);
        }
    }
}