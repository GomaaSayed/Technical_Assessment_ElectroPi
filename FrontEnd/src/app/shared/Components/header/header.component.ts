import { CommonModule } from '@angular/common';
import { Component, OnInit, OnDestroy } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';
import { Subject, takeUntil } from 'rxjs';
import { AuthService } from '../../services/auth-Service';

interface NotificationItem {
  id: string;
  title: string;
  message: string;
  type: string;
  isRead: boolean;
  createdAt: Date;
}

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss',
})
export class HeaderComponent implements OnInit, OnDestroy {
  role: string | null = null;

  notifications: NotificationItem[] = [];

  unreadCount = 0;

  private destroy$ = new Subject<void>();

  constructor(
    private router: Router,
    private authService: AuthService,
  ) {}

  ngOnInit(): void {
    // Listen for role changes immediately after login/logout
    this.authService.role$.pipe(takeUntil(this.destroy$)).subscribe((role) => {
      this.role = this.normalizeRole(role);

      // Reload notifications according to the new role
      this.loadNotifications();
    });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // =========================================================
  // Role
  // =========================================================

  private normalizeRole(role: string | null): string | null {
    if (!role) {
      return null;
    }

    switch (role.toLowerCase()) {
      case 'admin':
        return 'Admin';

      case 'supportagent':
        return 'SupportAgent';

      case 'customer':
        return 'Customer';

      default:
        return role;
    }
  }

  isAdmin(): boolean {
    return this.role?.toLowerCase() === 'admin';
  }

  isSupportAgent(): boolean {
    return this.role?.toLowerCase() === 'supportagent';
  }

  isCustomer(): boolean {
    return this.role?.toLowerCase() === 'customer';
  }

  // =========================================================
  // Notifications
  // =========================================================

  loadNotifications(): void {
    if (this.isAdmin()) {
      this.notifications = this.getAdminNotifications();
    } else if (this.isSupportAgent()) {
      this.notifications = this.getSupportAgentNotifications();
    } else if (this.isCustomer()) {
      this.notifications = this.getCustomerNotifications();
    } else {
      this.notifications = [];
    }

    this.updateUnreadCount();
  }

  private getAdminNotifications(): NotificationItem[] {
    return [
      {
        id: 'admin-1',
        title: 'New Ticket',
        message: 'A new support ticket has been created.',
        type: 'ticket',
        isRead: false,
        createdAt: new Date(),
      },
      {
        id: 'admin-2',
        title: 'Ticket Updated',
        message: 'A support ticket has been updated.',
        type: 'update',
        isRead: false,
        createdAt: new Date(),
      },
      {
        id: 'admin-3',
        title: 'New Customer',
        message: 'A new customer has registered.',
        type: 'user',
        isRead: true,
        createdAt: new Date(),
      },
    ];
  }

  private getSupportAgentNotifications(): NotificationItem[] {
    return [
      {
        id: 'agent-1',
        title: 'Ticket Assigned',
        message: 'A new ticket has been assigned to you.',
        type: 'assignment',
        isRead: false,
        createdAt: new Date(),
      },
      {
        id: 'agent-2',
        title: 'Ticket Updated',
        message: 'One of your assigned tickets has been updated.',
        type: 'update',
        isRead: false,
        createdAt: new Date(),
      },
      {
        id: 'agent-3',
        title: 'New Comment',
        message: 'A customer added a new comment to a ticket.',
        type: 'comment',
        isRead: true,
        createdAt: new Date(),
      },
    ];
  }

  private getCustomerNotifications(): NotificationItem[] {
    return [
      {
        id: 'customer-1',
        title: 'Ticket Created',
        message: 'Your support ticket has been created successfully.',
        type: 'ticket',
        isRead: false,
        createdAt: new Date(),
      },
      {
        id: 'customer-2',
        title: 'Ticket Updated',
        message: 'Your ticket status has been updated.',
        type: 'update',
        isRead: false,
        createdAt: new Date(),
      },
      {
        id: 'customer-3',
        title: 'New Comment',
        message: 'A support agent added a comment to your ticket.',
        type: 'comment',
        isRead: true,
        createdAt: new Date(),
      },
    ];
  }

  updateUnreadCount(): void {
    this.unreadCount = this.notifications.filter(
      (notification) => !notification.isRead,
    ).length;
  }

  markAsRead(notification: NotificationItem): void {
    notification.isRead = true;
    this.updateUnreadCount();
  }

  markAllAsRead(): void {
    this.notifications.forEach((notification) => (notification.isRead = true));

    this.updateUnreadCount();
  }

  // =========================================================
  // Notification Icons
  // =========================================================

  getNotificationIcon(type: string): string {
    switch (type) {
      case 'ticket':
        return 'bi-ticket-detailed';

      case 'assignment':
        return 'bi-person-check';

      case 'update':
        return 'bi-arrow-repeat';

      case 'comment':
        return 'bi-chat-left-text';

      case 'user':
        return 'bi-person-plus';

      default:
        return 'bi-bell';
    }
  }

  // =========================================================
  // Notification Colors
  // =========================================================

  getNotificationClass(type: string): string {
    // Admin
    if (this.isAdmin()) {
      switch (type) {
        case 'ticket':
          return 'notification-admin-ticket';

        case 'update':
          return 'notification-admin-update';

        case 'user':
          return 'notification-admin-user';

        default:
          return 'notification-admin';
      }
    }

    // Support Agent
    if (this.isSupportAgent()) {
      switch (type) {
        case 'assignment':
          return 'notification-agent-assignment';

        case 'update':
          return 'notification-agent-update';

        case 'comment':
          return 'notification-agent-comment';

        default:
          return 'notification-agent';
      }
    }

    // Customer
    if (this.isCustomer()) {
      switch (type) {
        case 'ticket':
          return 'notification-customer-ticket';

        case 'update':
          return 'notification-customer-update';

        case 'comment':
          return 'notification-customer-comment';

        default:
          return 'notification-customer';
      }
    }

    return 'notification-default';
  }

  // =========================================================
  // Navigation
  // =========================================================

  goToDashboard(): void {
    if (this.isAdmin()) {
      this.router.navigate(['/admin']);
      return;
    }

    if (this.isSupportAgent()) {
      this.router.navigate(['/supportagent']);
      return;
    }

    if (this.isCustomer()) {
      this.router.navigate(['/customer']);
      return;
    }

    this.router.navigate(['/login']);
  }

  // =========================================================
  // Logout
  // =========================================================

  logout(): void {
    this.authService.logout();

    this.notifications = [];
    this.unreadCount = 0;

    this.router.navigate(['/login']);
  }
}
