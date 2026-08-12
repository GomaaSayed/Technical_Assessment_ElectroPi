import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';

import { TicketService } from '../../Services/ticket.service';
import { TicketStatus } from '../../ENUM/Status';
import { TicketPriority } from '../../ENUM/Priorities';
import { CreateTimeEntryDTO } from '../../DTOs/CreateTimeEntryDTO';
import { TimeEntryDTO } from '../../DTOs/TimeEntryDTO';

@Component({
  selector: 'app-support-agent',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './support-agent.component.html',
  styleUrl: './support-agent.component.scss',
})
export class SupportAgentComponent implements OnInit {
  // =========================
  // Enums
  // =========================

  TicketStatus = TicketStatus;
  TicketPriority = TicketPriority;

  // =========================
  // Forms
  // =========================

  filterForm: FormGroup;

  // =========================
  // Data
  // =========================

  tickets: any[] = [];

  // =========================
  // UI State
  // =========================

  isLoadingTickets = false;

  successMessage = '';
  errorMessage = '';

  updatingTicketId: string | null = null;

  // =========================
  // Pagination
  // =========================

  currentPage = 1;
  pageSize = 5;
  totalCount = 0;

  get totalPages(): number {
    return Math.ceil(this.totalCount / this.pageSize);
  }

  get pages(): number[] {
    return Array.from({ length: this.totalPages }, (_, index) => index + 1);
  }

  get displayedFrom(): number {
    if (this.totalCount === 0) {
      return 0;
    }

    return (this.currentPage - 1) * this.pageSize + 1;
  }

  get displayedTo(): number {
    return Math.min(this.currentPage * this.pageSize, this.totalCount);
  }

  // =========================
  // Filter Options
  // =========================

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

  // =========================
  // Constructor
  // =========================

  constructor(
    private fb: FormBuilder,
    private ticketService: TicketService,
  ) {
    this.filterForm = this.fb.group({
      search: [''],
      status: [null],
      priority: [null],
      sortBy: [null],
      descending: [true],
    });
  }

  // =========================
  // Init
  // =========================

  ngOnInit(): void {
    this.loadMyTickets();
  }

  // =========================
  // Load Tickets
  // =========================

  loadMyTickets(): void {
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

    const sortBy = filters.sortBy?.trim() || undefined;

    const descending = filters.descending ?? true;

    this.ticketService
      .getMyTickets(
        search,
        status,
        priority,
        undefined,
        this.currentPage,
        this.pageSize,
        sortBy,
        descending,
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
          console.error('Failed to load support agent tickets:', error);

          this.tickets = [];

          this.totalCount = 0;

          this.isLoadingTickets = false;

          this.errorMessage =
            error?.error?.message ?? 'Failed to load your assigned tickets.';
        },
      });
  }

  // =========================
  // Refresh
  // =========================

  refresh(): void {
    this.loadMyTickets();
  }

  // =========================
  // Filters
  // =========================

  applyFilters(): void {
    this.currentPage = 1;

    this.loadMyTickets();
  }

  clearFilters(): void {
    this.filterForm.reset({
      search: '',
      status: null,
      priority: null,
      sortBy: null,
      descending: true,
    });

    this.currentPage = 1;

    this.loadMyTickets();
  }

  // =========================
  // Sorting
  // =========================

  sort(column: string): void {
    const currentSortBy = this.filterForm.get('sortBy')?.value;

    const currentDescending = this.filterForm.get('descending')?.value ?? true;

    if (currentSortBy === column) {
      this.filterForm.patchValue({
        descending: !currentDescending,
      });
    } else {
      this.filterForm.patchValue({
        sortBy: column,
        descending: true,
      });
    }

    this.currentPage = 1;

    this.loadMyTickets();
  }

  getSortIcon(field: string): string {
    const currentSort = this.filterForm.get('sortBy')?.value;

    if (currentSort !== field) {
      return 'bi-arrow-down-up text-muted';
    }

    const descending = this.filterForm.get('descending')?.value;

    return descending ? 'bi-arrow-down' : 'bi-arrow-up';
  }

  // =========================
  // Update Status
  // =========================

  updateStatus(ticket: any, status: number): void {
    if (ticket.status === status || this.updatingTicketId === ticket.id) {
      return;
    }

    const oldStatus = ticket.status;

    this.updatingTicketId = ticket.id;

    this.clearMessages();

    this.ticketService.updateTicketStatus(ticket.id, status).subscribe({
      next: () => {
        ticket.status = status;

        this.updatingTicketId = null;

        this.successMessage = 'Ticket status updated successfully.';
      },

      error: (error) => {
        console.error('Failed to update ticket status:', error);

        // Restore previous value
        ticket.status = oldStatus;

        this.updatingTicketId = null;

        this.errorMessage =
          error?.error?.message ?? 'Failed to update ticket status.';
      },
    });
  }
  // =========================
  // Comments
  // =========================

  commentText: { [ticketId: string]: string } = {};

  showCommentBox: { [ticketId: string]: boolean } = {};

  commentingTicketId: string | null = null;

  comments: { [ticketId: string]: any[] } = {};
  loadingComments: {
    [ticketId: string]: boolean;
  } = {};
  hasCommentText(ticketId: string): boolean {
    return !!this.commentText[ticketId]?.trim();
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

    // Load previous comments when opening
    this.loadComments(ticketId);
  }
  loadComments(ticketId: string): void {
    this.loadingComments[ticketId] = true;

    this.ticketService.getTicketComments(ticketId).subscribe({
      next: (response: any) => {
        console.log('COMMENTS RESPONSE:', response);
        console.log('IS ARRAY:', Array.isArray(response));

        this.comments[ticketId] = Array.isArray(response)
          ? response
          : (response?.items ?? response?.data ?? []);

        console.log('COMMENTS STORED:', this.comments[ticketId]);

        this.loadingComments[ticketId] = false;
      },

      error: (error) => {
        console.error('Failed to load comments:', error);

        this.comments[ticketId] = [];
        this.loadingComments[ticketId] = false;
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

        // Reload comments
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
  // =========================
  // Assign Ticket
  // =========================

  assignTicket(ticket: any, agentId: string): void {
    if (!agentId || this.updatingTicketId === ticket.id) {
      return;
    }

    this.updatingTicketId = ticket.id;

    this.clearMessages();

    this.ticketService.assignTicket(ticket.id, agentId).subscribe({
      next: () => {
        ticket.assignedAgentId = agentId;

        this.updatingTicketId = null;

        this.successMessage = 'Ticket assigned successfully.';
      },

      error: (error) => {
        console.error('Failed to assign ticket:', error);

        this.updatingTicketId = null;

        this.errorMessage = error?.error?.message ?? 'Failed to assign ticket.';
      },
    });
  }

  // =========================
  // Unassign Ticket
  // =========================

  unassignTicket(ticket: any): void {
    if (!ticket.assignedAgentId || this.updatingTicketId === ticket.id) {
      return;
    }

    this.updatingTicketId = ticket.id;

    this.clearMessages();

    this.ticketService.unassignTicket(ticket.id).subscribe({
      next: () => {
        ticket.assignedAgentId = null;

        this.updatingTicketId = null;

        this.successMessage = 'Ticket unassigned successfully.';
      },

      error: (error) => {
        console.error('Failed to unassign ticket:', error);

        this.updatingTicketId = null;

        this.errorMessage =
          error?.error?.message ?? 'Failed to unassign ticket.';
      },
    });
  }

  // =========================
  // Pagination
  // =========================

  goToPage(page: number): void {
    if (
      page < 1 ||
      page > this.totalPages ||
      page === this.currentPage ||
      this.isLoadingTickets
    ) {
      return;
    }

    this.currentPage = page;

    this.loadMyTickets();
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages && !this.isLoadingTickets) {
      this.currentPage++;

      this.loadMyTickets();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1 && !this.isLoadingTickets) {
      this.currentPage--;

      this.loadMyTickets();
    }
  }

  // =========================
  // Labels
  // =========================

  getPriorityLabel(priority: number): string {
    return (
      this.priorities.find((x) => x.value === priority)?.label ?? 'Unknown'
    );
  }

  getStatusLabel(status: number): string {
    return this.statuses.find((x) => x.value === status)?.label ?? 'Unknown';
  }

  // =========================
  // CSS Helpers
  // =========================

  getPriorityClass(priority: number): string {
    switch (priority) {
      case TicketPriority.Low:
        return 'bg-success';

      case TicketPriority.Medium:
        return 'bg-warning text-dark';

      case TicketPriority.High:
        return 'bg-danger';

      default:
        return 'bg-secondary';
    }
  }

  getStatusClass(status: number): string {
    switch (status) {
      case TicketStatus.Open:
        return 'bg-primary';

      case TicketStatus.InProgress:
        return 'bg-warning text-dark';

      case TicketStatus.Resolved:
        return 'bg-success';

      case TicketStatus.Closed:
        return 'bg-secondary';

      default:
        return 'bg-secondary';
    }
  }

  // =========================
  // Helpers
  // =========================

  isUpdating(ticket: any): boolean {
    return this.updatingTicketId === ticket.id;
  }

  clearMessages(): void {
    this.successMessage = '';
    this.errorMessage = '';
  }
  timeEntryTicketId: string | null = null;

  timeEntry = {
    workDate: '',
    durationMinutes: 0,
    description: '',
  };

  openTimeEntry(ticket: any): void {
    const ticketId = ticket.id;

    this.clearMessages();

    // Toggle
    if (this.timeEntryTicketId === ticketId) {
      this.timeEntryTicketId = null;
      return;
    }

    this.timeEntryTicketId = ticketId;

    // Reset form
    this.timeEntry = {
      workDate: new Date().toISOString().split('T')[0],
      durationMinutes: 0,
      description: '',
    };

    this.loadTimeEntries(ticketId);
  }
  loadTimeEntries(ticketId: string): void {
    this.loadingTimeEntries[ticketId] = true;

    this.ticketService.getTimeEntries(ticketId).subscribe({
      next: (response) => {
        this.timeEntries[ticketId] = response ?? [];
        this.loadingTimeEntries[ticketId] = false;
      },

      error: (error) => {
        console.error('Failed to load time entries:', error);

        this.timeEntries[ticketId] = [];
        this.loadingTimeEntries[ticketId] = false;

        this.errorMessage =
          error?.error?.message ?? 'Failed to load time entries.';
      },
    });
  }
  cancelTimeEntry(): void {
    this.timeEntryTicketId = null;

    this.timeEntry = {
      workDate: '',
      durationMinutes: 0,
      description: '',
    };
  }

  createTimeEntry(ticket: any): void {
    if (
      !this.timeEntry.workDate ||
      this.timeEntry.durationMinutes <= 0 ||
      !this.timeEntry.description.trim()
    ) {
      this.errorMessage = 'Work date, duration and description are required.';

      return;
    }

    this.updatingTicketId = ticket.id;
    this.clearMessages();

    const dto: CreateTimeEntryDTO = {
      workDate: this.timeEntry.workDate,
      durationMinutes: Number(this.timeEntry.durationMinutes),
      description: this.timeEntry.description.trim(),
    };

    this.ticketService.createTimeEntry(ticket.id, dto).subscribe({
      next: () => {
        this.updatingTicketId = null;
        this.timeEntryTicketId = null;

        this.successMessage = 'Time entry added successfully.';

        this.timeEntry = {
          workDate: '',
          durationMinutes: 0,
          description: '',
        };
        // Reload existing entries
        this.loadTimeEntries(ticket.id);
      },

      error: (error) => {
        console.error('Failed to create time entry:', error);

        this.updatingTicketId = null;

        this.errorMessage =
          error?.error?.message ?? 'Failed to add time entry.';
      },
    });
  }
  timeEntries: { [ticketId: string]: TimeEntryDTO[] } = {};

  loadingTimeEntries: {
    [ticketId: string]: boolean;
  } = {};
}
