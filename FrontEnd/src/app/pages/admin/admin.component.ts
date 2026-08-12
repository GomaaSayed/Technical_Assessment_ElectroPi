import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';

import { TicketService } from '../../Services/ticket.service';
import { UserService } from '../../Services/user.service';

import { TicketStatus } from '../../ENUM/Status';
import { TicketPriority } from '../../ENUM/Priorities';

@Component({
  selector: 'app-admin',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss',
})
export class AdminComponent implements OnInit {
  // =========================================================
  // Enums
  // =========================================================

  TicketStatus = TicketStatus;
  TicketPriority = TicketPriority;

  // =========================================================
  // Form
  // =========================================================

  filterForm: FormGroup;

  // =========================================================
  // Data
  // =========================================================

  tickets: any[] = [];

  agents: any[] = [];

  // =========================================================
  // Pagination
  // =========================================================

  currentPage = 1;
  pageSize = 5;
  totalCount = 0;

  // =========================================================
  // Sorting
  // =========================================================

  sortBy: string | undefined = undefined;
  descending = true;

  // =========================================================
  // Loading / Processing
  // =========================================================

  isLoadingTickets = false;
  isLoadingAgents = false;
  isProcessing = false;

  // =========================================================
  // Messages
  // =========================================================

  successMessage = '';
  errorMessage = '';

  // =========================================================
  // Filter Options
  // =========================================================

  priorities = [
    {
      value: TicketPriority.Low,
      label: 'Low',
    },
    {
      value: TicketPriority.Medium,
      label: 'Medium',
    },
    {
      value: TicketPriority.High,
      label: 'High',
    },
  ];

  statuses = [
    {
      value: TicketStatus.Open,
      label: 'Open',
    },
    {
      value: TicketStatus.InProgress,
      label: 'In Progress',
    },
    {
      value: TicketStatus.Resolved,
      label: 'Resolved',
    },
    {
      value: TicketStatus.Closed,
      label: 'Closed',
    },
  ];

  // =========================================================
  // Constructor
  // =========================================================

  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService,
    private userService: UserService,
  ) {
    this.filterForm = this.fb.group({
      search: [''],
      status: [null],
      priority: [null],
    });
  }

  // =========================================================
  // Init
  // =========================================================

  ngOnInit(): void {
    this.loadAgents();
    this.loadTickets();
  }

  // =========================================================
  // Load Support Agents
  // =========================================================

  loadAgents(): void {
    this.isLoadingAgents = true;

    this.userService.getAgents().subscribe({
      next: (response) => {
        this.agents = response ?? [];

        console.log('Support Agents:', this.agents);

        this.isLoadingAgents = false;
      },

      error: (error) => {
        console.error('Failed to load agents:', error);

        this.agents = [];

        this.isLoadingAgents = false;

        this.errorMessage =
          error?.error?.message ?? 'Failed to load support agents.';
      },
    });
  }

  // =========================================================
  // Load All Tickets
  // =========================================================

  loadTickets(): void {
    this.isLoadingTickets = true;
    this.errorMessage = '';

    const filters = this.filterForm.value;

    const search = filters.search?.trim() || undefined;

    const status =
      filters.status !== null &&
      filters.status !== undefined &&
      filters.status !== ''
        ? Number(filters.status)
        : undefined;

    const priority =
      filters.priority !== null &&
      filters.priority !== undefined &&
      filters.priority !== ''
        ? Number(filters.priority)
        : undefined;

    this.ticketService
      .getTickets(
        search,
        status,
        priority,
        undefined, // AssignedAgentId
        this.currentPage,
        this.pageSize,
        this.sortBy,
        this.descending,
      )
      .subscribe({
        next: (response) => {
          this.tickets = response?.items ?? [];

          this.totalCount = response?.totalCount ?? 0;

          this.currentPage = response?.pageNumber ?? this.currentPage;

          this.pageSize = response?.pageSize ?? this.pageSize;

          this.isLoadingTickets = false;
        },

        error: (error) => {
          console.error('Failed to load tickets:', error);

          this.tickets = [];

          this.totalCount = 0;

          this.isLoadingTickets = false;

          this.errorMessage =
            error?.error?.message ?? 'Failed to load tickets.';
        },
      });
  }

  // =========================================================
  // Apply Filters
  // =========================================================

  applyFilters(): void {
    this.currentPage = 1;

    this.loadTickets();
  }

  // =========================================================
  // Clear Filters
  // =========================================================

  clearFilters(): void {
    this.filterForm.reset({
      search: '',
      status: null,
      priority: null,
    });

    this.sortBy = undefined;

    this.descending = true;

    this.currentPage = 1;

    this.loadTickets();
  }

  // =========================================================
  // Sorting
  // =========================================================

  sort(column: string): void {
    if (this.sortBy === column) {
      this.descending = !this.descending;
    } else {
      this.sortBy = column;
      this.descending = true;
    }

    this.currentPage = 1;

    this.loadTickets();
  }

  // =========================================================
  // Sort Icon
  // =========================================================

  getSortIcon(column: string): string {
    if (this.sortBy !== column) {
      return 'bi-arrow-down-up text-muted';
    }

    return this.descending ? 'bi-arrow-down' : 'bi-arrow-up';
  }

  // =========================================================
  // Pagination
  // =========================================================

  get totalPages(): number {
    if (this.pageSize <= 0) {
      return 0;
    }

    return Math.ceil(this.totalCount / this.pageSize);
  }

  get pages(): number[] {
    const totalPages = this.totalPages;

    if (totalPages <= 0) {
      return [];
    }

    return Array.from({ length: totalPages }, (_, index) => index + 1);
  }

  goToPage(page: number): void {
    if (page < 1 || page > this.totalPages || page === this.currentPage) {
      return;
    }

    this.currentPage = page;

    this.loadTickets();
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;

      this.loadTickets();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;

      this.loadTickets();
    }
  }

  // =========================================================
  // Change Page Size
  // =========================================================

  changePageSize(size: number): void {
    this.pageSize = Number(size);

    this.currentPage = 1;

    this.loadTickets();
  }

  // =========================================================
  // Assign Ticket
  // =========================================================

  assignTicket(ticketId: string, agentId: string): void {
    // Nothing selected
    if (!agentId) {
      return;
    }

    this.isProcessing = true;

    this.successMessage = '';
    this.errorMessage = '';

    this.ticketService.assignTicket(ticketId, agentId).subscribe({
      next: () => {
        this.successMessage = 'Ticket assigned successfully.';

        this.isProcessing = false;

        this.loadTickets();
      },

      error: (error) => {
        console.error('Failed to assign ticket:', error);

        this.isProcessing = false;

        this.errorMessage = error?.error?.message ?? 'Failed to assign ticket.';
      },
    });
  }

  // =========================================================
  // Unassign Ticket
  // =========================================================

  unassignTicket(ticketId: string): void {
    const confirmed = confirm('Are you sure you want to unassign this ticket?');

    if (!confirmed) {
      return;
    }

    this.isProcessing = true;

    this.successMessage = '';
    this.errorMessage = '';

    this.ticketService.unassignTicket(ticketId).subscribe({
      next: () => {
        this.successMessage = 'Ticket unassigned successfully.';

        this.isProcessing = false;

        this.loadTickets();
      },

      error: (error) => {
        console.error('Failed to unassign ticket:', error);

        this.isProcessing = false;

        this.errorMessage =
          error?.error?.message ?? 'Failed to unassign ticket.';
      },
    });
  }

  // =========================================================
  // Update Status
  // =========================================================

  updateStatus(ticketId: string, status: number): void {
    this.isProcessing = true;

    this.successMessage = '';
    this.errorMessage = '';

    this.ticketService.updateTicketStatus(ticketId, status).subscribe({
      next: () => {
        this.successMessage = 'Ticket status updated successfully.';

        this.isProcessing = false;

        this.loadTickets();
      },

      error: (error) => {
        console.error('Failed to update ticket status:', error);

        this.isProcessing = false;

        this.errorMessage =
          error?.error?.message ?? 'Failed to update ticket status.';
      },
    });
  }

  // =========================================================
  // Update Priority
  // =========================================================

  updatePriority(ticketId: string, priority: number): void {
    this.isProcessing = true;

    this.successMessage = '';
    this.errorMessage = '';

    this.ticketService.updateTicketPriority(ticketId, priority).subscribe({
      next: () => {
        this.successMessage = 'Ticket priority updated successfully.';

        this.isProcessing = false;

        this.loadTickets();
      },

      error: (error) => {
        console.error('Failed to update ticket priority:', error);

        this.isProcessing = false;

        this.errorMessage =
          error?.error?.message ?? 'Failed to update ticket priority.';
      },
    });
  }

  // =========================================================
  // Delete Ticket
  // =========================================================

  deleteTicket(ticketId: string): void {
    const confirmed = confirm('Are you sure you want to delete this ticket?');

    if (!confirmed) {
      return;
    }

    this.isProcessing = true;

    this.successMessage = '';
    this.errorMessage = '';

    this.ticketService.deleteTicket(ticketId).subscribe({
      next: () => {
        this.successMessage = 'Ticket deleted successfully.';

        this.isProcessing = false;

        if (this.tickets.length === 1 && this.currentPage > 1) {
          this.currentPage--;
        }

        this.loadTickets();
      },

      error: (error) => {
        console.error('Failed to delete ticket:', error);

        this.isProcessing = false;

        this.errorMessage = error?.error?.message ?? 'Failed to delete ticket.';
      },
    });
  }

  // =========================================================
  // Get Agent Name
  // =========================================================

  getAgentName(agentId: string): string {
    const agent = this.agents.find((x) => x.id === agentId);

    if (!agent) {
      return 'Unknown Agent';
    }

    return (
      agent.fullName ??
      agent.name ??
      agent.userName ??
      agent.email ??
      'Unknown Agent'
    );
  }

  // =========================================================
  // Get Priority Label
  // =========================================================

  getPriorityLabel(priority: number): string {
    return TicketPriority[priority] ?? 'Unknown';
  }

  // =========================================================
  // Get Status Label
  // =========================================================

  getStatusLabel(status: number): string {
    return TicketStatus[status] ?? 'Unknown';
  }
  // =========================================================
  // Comments
  // =========================================================

  commentText: { [ticketId: string]: string } = {};

  showCommentBox: { [ticketId: string]: boolean } = {};

  commentingTicketId: string | null = null;

  comments: { [ticketId: string]: any[] } = {};

  loadingComments: { [ticketId: string]: boolean } = {};
  // =========================================================
  // Comments
  // =========================================================

  hasCommentText(ticketId: string): boolean {
    return !!this.commentText[ticketId]?.trim();
  }

  getComments(ticketId: string): any[] {
    return this.comments[ticketId] ?? [];
  }

  toggleCommentBox(ticket: any): void {
    const ticketId = ticket.id;

    const isOpening = !this.showCommentBox[ticketId];

    this.showCommentBox[ticketId] = isOpening;

    this.clearMessages();

    if (!isOpening) {
      this.commentText[ticketId] = '';
      return;
    }

    this.loadComments(ticketId);
  }

  loadComments(ticketId: string): void {
    this.loadingComments[ticketId] = true;

    this.ticketService.getTicketComments(ticketId).subscribe({
      next: (response) => {
        this.comments[ticketId] = response ?? [];

        this.loadingComments[ticketId] = false;
      },

      error: (error) => {
        console.error('Failed to load comments:', error);

        this.comments[ticketId] = [];

        this.loadingComments[ticketId] = false;

        this.errorMessage = error?.error?.message ?? 'Failed to load comments.';
      },
    });
  }

  addComment(ticket: any): void {
    const ticketId = ticket.id;

    const content = this.commentText[ticketId]?.trim() || '';

    if (!content) {
      this.errorMessage = 'Comment cannot be empty.';
      return;
    }

    if (this.commentingTicketId === ticketId) {
      return;
    }

    this.commentingTicketId = ticketId;

    this.clearMessages();

    this.ticketService.addComment(ticketId, content).subscribe({
      next: () => {
        this.commentingTicketId = null;

        this.commentText[ticketId] = '';

        this.successMessage = 'Comment added successfully.';

        // Reload comments after adding
        this.loadComments(ticketId);
      },

      error: (error) => {
        console.error('Failed to add comment:', error);

        this.commentingTicketId = null;

        this.errorMessage = error?.error?.message ?? 'Failed to add comment.';
      },
    });
  }

  isCommenting(ticket: any): boolean {
    return this.commentingTicketId === ticket.id;
  }

  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
}
