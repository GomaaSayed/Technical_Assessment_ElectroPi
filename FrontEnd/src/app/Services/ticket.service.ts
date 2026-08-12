import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, map, switchMap, take } from 'rxjs';

import { ConfigService } from '../shared/services/config-service';
import { UpdateTicketDTO } from '../DTOs/UpdateTicketDTO';
import { CreateTicketDTO } from '../DTOs/CreateTicketDTO';
import { CreateTimeEntryDTO } from '../DTOs/CreateTimeEntryDTO';
import { TimeEntryDTO } from '../DTOs/TimeEntryDTO';

@Injectable({
  providedIn: 'root',
})
export class TicketService {
  constructor(
    private http: HttpClient,
    private configService: ConfigService,
  ) {}

  private getBaseUrl(): Observable<string> {
    return this.configService.getConfig().pipe(
      take(1),
      map((config) => config.baseUrl),
    );
  }

  getTicketById(ticketId: string): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.get<any>(`${baseUrl}Ticket/${ticketId}`),
      ),
    );
  }

  updateTicket(ticketId: string, ticket: UpdateTicketDTO): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.put<any>(`${baseUrl}Ticket/${ticketId}`, ticket),
      ),
    );
  }

  deleteTicket(ticketId: string): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.delete<any>(`${baseUrl}Ticket/${ticketId}`),
      ),
    );
  }

  getTickets(
    search?: string,
    status?: number,
    priority?: number,
    assignedAgentId?: string,
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    descending: boolean = false,
  ): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) => {
        const params: any = {
          PageNumber: pageNumber,
          PageSize: pageSize,
          Descending: descending,
        };

        if (search) {
          params.Search = search;
        }

        if (status !== undefined) {
          params.Status = status;
        }

        if (priority !== undefined) {
          params.Priority = priority;
        }

        if (assignedAgentId) {
          params.AssignedAgentId = assignedAgentId;
        }

        if (sortBy) {
          params.SortBy = sortBy;
        }

        return this.http.get<any>(`${baseUrl}Ticket`, {
          params,
        });
      }),
    );
  }
  getCustomerTickets(
    search?: string,
    status?: number,
    priority?: number,
    assignedAgentId?: string,
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    descending: boolean = false,
  ): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) => {
        const params: any = {
          PageNumber: pageNumber,
          PageSize: pageSize,
          Descending: descending,
        };

        if (search) {
          params.Search = search;
        }

        if (status !== undefined) {
          params.Status = status;
        }

        if (priority !== undefined) {
          params.Priority = priority;
        }

        if (assignedAgentId) {
          params.AssignedAgentId = assignedAgentId;
        }

        if (sortBy) {
          params.SortBy = sortBy;
        }

        return this.http.get<any>(`${baseUrl}Ticket/customer-tickets`, {
          params,
        });
      }),
    );
  }
  getMyTickets(
    search?: string,
    status?: number,
    priority?: number,
    assignedAgentId?: string,
    pageNumber: number = 1,
    pageSize: number = 10,
    sortBy?: string,
    descending: boolean = false,
  ): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) => {
        const params: any = {
          PageNumber: pageNumber,
          PageSize: pageSize,
          Descending: descending,
        };

        if (search) {
          params.Search = search;
        }

        if (status !== undefined) {
          params.Status = status;
        }

        if (priority !== undefined) {
          params.Priority = priority;
        }

        if (assignedAgentId) {
          params.AssignedAgentId = assignedAgentId;
        }

        if (sortBy) {
          params.SortBy = sortBy;
        }

        return this.http.get<any>(`${baseUrl}Ticket/my-tickets`, {
          params,
        });
      }),
    );
  }
  createTicket(ticket: CreateTicketDTO): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) => this.http.post<any>(`${baseUrl}Ticket`, ticket)),
    );
  }

  assignTicket(ticketId: string, agentId: string): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.put<any>(
          `${baseUrl}Ticket/${ticketId}/assign/${agentId}`,
          null,
        ),
      ),
    );
  }

  unassignTicket(ticketId: string): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.delete<any>(`${baseUrl}Ticket/${ticketId}/assign`),
      ),
    );
  }

  updateTicketStatus(ticketId: string, status: number): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.patch<any>(`${baseUrl}Ticket/${ticketId}/status`, null, {
          params: {
            status: status,
          },
        }),
      ),
    );
  }

  updateTicketPriority(ticketId: string, priority: number): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.patch<any>(`${baseUrl}Ticket/${ticketId}/priority`, null, {
          params: {
            priority: priority,
          },
        }),
      ),
    );
  }
  getTimeEntries(ticketId: string): Observable<TimeEntryDTO[]> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.get<TimeEntryDTO[]>(
          `${baseUrl}Ticket/${ticketId}/time-entries`,
        ),
      ),
    );
  }
  createTimeEntry(
    ticketId: string,
    timeEntry: CreateTimeEntryDTO,
  ): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.post<any>(
          `${baseUrl}Ticket/${ticketId}/time-entries`,
          timeEntry,
        ),
      ),
    );
  }
  addComment(ticketId: string, content: string): Observable<any> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.post<any>(`${baseUrl}Ticket/${ticketId}/comments`, {
          content: content,
        }),
      ),
    );
  }
  getTicketComments(ticketId: string): Observable<any[]> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.get<any[]>(`${baseUrl}Ticket/${ticketId}/comments`),
      ),
    );
  }
}
