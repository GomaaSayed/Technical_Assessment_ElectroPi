import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  FormGroup,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';

import { TicketService } from '../../Services/ticket.service';
import { TicketStatus } from '../../ENUM/Status';
import { TicketPriority } from '../../ENUM/Priorities';

@Component({
  selector: 'app-customer',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './customer.component.html',
  styleUrl: './customer.component.scss',
})
export class CustomerComponent implements OnInit {
  // =========================
  // Enums
  // =========================

  TicketStatus = TicketStatus;
  TicketPriority = TicketPriority;

  // =========================
  // Forms
  // =========================

  ticketForm: FormGroup;
  filterForm: FormGroup;

  // =========================
  // Data
  // =========================

  tickets: any[] = [];

  // =========================
  // UI State
  // =========================

  isSubmitting = false;
  isLoadingTickets = false;

  showCreateTicketModal = false;

  successMessage = '';
  errorMessage = '';

  // =========================
  // Sorting
  // =========================

  /**
   * Backend SortBy values:
   *
   * 0 = Default
   * 1 = Title
   * 2 = Priority
   * 3 = Status
   * 4 = CreatedAt
   */
  sortBy: number | undefined = undefined;
  descending = true;
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
    // Create Ticket Form
    this.ticketForm = this.fb.group({
      title: ['', [Validators.required, Validators.maxLength(200)]],

      description: ['', [Validators.required]],

      priority: [TicketPriority.Low, [Validators.required]],
    });

    // Filters only
    this.filterForm = this.fb.group({
      search: [''],

      status: [null],

      priority: [null],
    });
  }

  // =========================
  // Init
  // =========================

  ngOnInit(): void {
    this.loadCustomerTickets();
  }

  // =========================
  // Create Ticket Modal
  // =========================

  openCreateTicketModal(): void {
    this.successMessage = '';
    this.errorMessage = '';

    this.resetForm();

    this.showCreateTicketModal = true;
  }

  closeCreateTicketModal(): void {
    if (this.isSubmitting) {
      return;
    }

    this.showCreateTicketModal = false;
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
  // Create Ticket
  // =========================

  createTicket(): void {
    this.successMessage = '';
    this.errorMessage = '';

    if (this.ticketForm.invalid) {
      this.ticketForm.markAllAsTouched();
      return;
    }

    this.isSubmitting = true;

    const ticket = {
      title: this.ticketForm.value.title,
      description: this.ticketForm.value.description,
      priority: Number(this.ticketForm.value.priority),
    };

    this.ticketService.createTicket(ticket).subscribe({
      next: () => {
        this.isSubmitting = false;

        this.showCreateTicketModal = false;

        this.successMessage = 'Your ticket has been created successfully.';

        this.resetForm();
        this.currentPage = 1;
        this.loadCustomerTickets();
      },

      error: (error) => {
        console.error('Failed to create ticket:', error);

        this.isSubmitting = false;

        this.errorMessage =
          error?.error?.message ?? 'Failed to create ticket. Please try again.';
      },
    });
  }

  // =========================
  // Load Customer Tickets
  // =========================

  loadCustomerTickets(): void {
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
      .getCustomerTickets(
        search,
        status,
        priority,
        undefined, // AssignedAgentId
        this.currentPage, // PageNumber
        this.pageSize, // PageSize
        this.sortBy, // SortBy
        this.descending, // Descending
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
          console.error('Failed to load customer tickets:', error);

          this.tickets = [];

          this.totalCount = 0;

          this.isLoadingTickets = false;

          this.errorMessage =
            error?.error?.message ?? 'Failed to load your tickets.';
        },
      });
  }

  // =========================
  // Apply Filters
  // =========================

  applyFilters(): void {
    this.currentPage = 1;

    this.loadCustomerTickets();
  }
  // =========================
  // Clear Filters
  // =========================

  clearFilters(): void {
    this.filterForm.reset({
      search: '',
      status: null,
      priority: null,
    });

    this.sortBy = undefined;
    this.descending = true;

    this.currentPage = 1;

    this.loadCustomerTickets();
  }

  // =========================
  // Table Sorting
  // =========================

  sort(field: string): void {
    const currentSort = this.filterForm.get('sortBy')?.value;
    const currentDescending = this.filterForm.get('descending')?.value ?? true;

    if (currentSort === field) {
      this.filterForm.patchValue({
        descending: !currentDescending,
      });
    } else {
      this.filterForm.patchValue({
        sortBy: field,
        descending: false,
      });
    }

    this.loadCustomerTickets();
  }

  getSortIcon(field: string): string {
    const currentSort = this.filterForm.get('sortBy')?.value;

    if (currentSort !== field) {
      return 'bi-arrow-down-up text-muted';
    }

    const descending = this.filterForm.get('descending')?.value;

    return descending ? 'bi-arrow-down' : 'bi-arrow-up';
  }
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

    this.loadCustomerTickets();
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;

      this.loadCustomerTickets();
    }
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;

      this.loadCustomerTickets();
    }
  }
  // =========================
  // Labels
  // =========================

  getPriorityLabel(priority: number): string {
    return TicketPriority[priority] ?? 'Unknown';
  }

  getStatusLabel(status: number): string {
    return TicketStatus[status] ?? 'Unknown';
  }

  // =========================
  // Reset Ticket Form
  // =========================

  resetForm(): void {
    this.ticketForm.reset({
      title: '',
      description: '',
      priority: TicketPriority.Low,
    });

    this.ticketForm.markAsPristine();
    this.ticketForm.markAsUntouched();
  }

  // =========================
  // Form Getters
  // =========================

  get title() {
    return this.ticketForm.get('title');
  }

  get description() {
    return this.ticketForm.get('description');
  }

  get priority() {
    return this.ticketForm.get('priority');
  }
}
