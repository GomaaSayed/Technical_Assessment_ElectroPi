import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { TicketService } from '../../Services/ticket.service';

@Component({
  selector: 'app-support-agent',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './support-agent.component.html',
  styleUrl: './support-agent.component.scss',
})
export class SupportAgentComponent implements OnInit {
  tickets: any[] = [];

  isLoading = false;

  search = '';
  selectedStatus?: number;
  selectedPriority?: number;

  pageNumber = 1;
  pageSize = 10;

  statuses = [
    { value: 0, label: 'Open' },
    { value: 1, label: 'In Progress' },
    { value: 2, label: 'Resolved' },
    { value: 3, label: 'Closed' },
  ];

  priorities = [
    { value: 0, label: 'Low' },
    { value: 1, label: 'Medium' },
    { value: 2, label: 'High' },
  ];

  constructor(private ticketService: TicketService) {}

  ngOnInit(): void {
    this.loadTickets();
  }

  loadTickets(): void {
    this.isLoading = true;

    this.ticketService
      .getTickets(
        this.search || undefined,
        this.selectedStatus,
        this.selectedPriority,
        undefined,
        this.pageNumber,
        this.pageSize,
        undefined,
        true,
      )
      .subscribe({
        next: (response) => {
          this.tickets = response.items ?? response;
          this.isLoading = false;
        },
        error: (error) => {
          console.error('Failed to load tickets:', error);
          this.isLoading = false;
        },
      });
  }

  searchTickets(): void {
    this.pageNumber = 1;
    this.loadTickets();
  }

  clearFilters(): void {
    this.search = '';
    this.selectedStatus = undefined;
    this.selectedPriority = undefined;
    this.pageNumber = 1;

    this.loadTickets();
  }

  updateStatus(ticket: any, status: number): void {
    this.ticketService.updateTicketStatus(ticket.id, status).subscribe({
      next: () => {
        ticket.status = status;
      },
      error: (error) => {
        console.error('Failed to update ticket status:', error);
      },
    });
  }

  updatePriority(ticket: any, priority: number): void {
    this.ticketService.updateTicketPriority(ticket.id, priority).subscribe({
      next: () => {
        ticket.priority = priority;
      },
      error: (error) => {
        console.error('Failed to update ticket priority:', error);
      },
    });
  }

  assignTicket(ticket: any, agentId: string): void {
    this.ticketService.assignTicket(ticket.id, agentId).subscribe({
      next: () => {
        ticket.assignedAgentId = agentId;
      },
      error: (error) => {
        console.error('Failed to assign ticket:', error);
      },
    });
  }

  unassignTicket(ticket: any): void {
    this.ticketService.unassignTicket(ticket.id).subscribe({
      next: () => {
        ticket.assignedAgentId = null;
      },
      error: (error) => {
        console.error('Failed to unassign ticket:', error);
      },
    });
  }

  getStatusLabel(status: number): string {
    return this.statuses.find((x) => x.value === status)?.label ?? 'Unknown';
  }

  getPriorityLabel(priority: number): string {
    return (
      this.priorities.find((x) => x.value === priority)?.label ?? 'Unknown'
    );
  }
}
