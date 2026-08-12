import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { Router, RouterLink, RouterLinkActive } from '@angular/router';

import { Subject, takeUntil } from 'rxjs';

import { AuthService } from '../../services/auth-Service';

import {
  NotificationSignalRService,
  NotificationItem,
} from '../../services/notification-signalr.service';

import { NotificationService } from '../../services/notification.service';

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
    private notificationSignalRService: NotificationSignalRService,
    private notificationService: NotificationService,
  ) {}

  // =========================================================
  // Lifecycle
  // =========================================================

  ngOnInit(): void {
    // =======================================================
    // Role changes
    // =======================================================

    this.authService.role$
      .pipe(takeUntil(this.destroy$))
      .subscribe(async (role) => {
        this.role = this.normalizeRole(role);

        // Clear old data
        this.notifications = [];
        this.unreadCount = 0;

        // ===================================================
        // Support Agent only
        // ===================================================

        if (this.isSupportAgent()) {
          // Load existing notifications from database
          this.loadNotifications();

          // Start real-time SignalR
          await this.startSupportAgentNotifications();
        } else {
          // Stop SignalR for Admin / Customer
          await this.notificationSignalRService.stopConnection();
        }
      });

    // =======================================================
    // Real-time notifications
    // =======================================================

    this.notificationSignalRService.notification$
      .pipe(takeUntil(this.destroy$))
      .subscribe((notification) => {
        // SignalR notifications are only for SupportAgent
        if (!this.isSupportAgent()) {
          return;
        }

        console.log('New real-time notification:', notification);

        // Prevent duplicate notification
        const exists = this.notifications.some((x) => x.id === notification.id);

        if (exists) {
          return;
        }

        // Add newest notification at the beginning
        this.notifications.unshift(notification);

        // Increment unread count only if notification is unread
        if (!notification.isRead) {
          this.unreadCount++;
        }
      });
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();

    this.notificationSignalRService.stopConnection();
  }

  // =========================================================
  // Load Notifications
  // =========================================================

  private loadNotifications(): void {
    if (!this.isSupportAgent()) {
      return;
    }

    this.notificationService
      .getMyNotifications()
      .pipe(takeUntil(this.destroy$))
      .subscribe({
        next: (notifications) => {
          console.log('Loaded notifications:', notifications);

          // Newest first
          this.notifications = notifications.sort(
            (a, b) =>
              new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime(),
          );

          // Calculate unread notifications
          this.updateUnreadCount();
        },

        error: (error) => {
          console.error('Failed to load notifications:', error);

          this.notifications = [];
          this.unreadCount = 0;
        },
      });
  }

  // =========================================================
  // SignalR
  // =========================================================

  private async startSupportAgentNotifications(): Promise<void> {
    try {
      await this.notificationSignalRService.startConnection();

      console.log('Support Agent notification SignalR started successfully.');
    } catch (error) {
      console.error(
        'Failed to start Support Agent notification SignalR:',
        error,
      );
    }
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

  updateUnreadCount(): void {
    this.unreadCount = this.notifications.filter(
      (notification) => !notification.isRead,
    ).length;
  }

  markAsRead(notification: NotificationItem): void {
    if (notification.isRead) {
      return;
    }

    notification.isRead = true;

    this.updateUnreadCount();

    // TODO:
    // Call backend endpoint here later
    // to persist IsRead = true in database.
  }

  markAllAsRead(): void {
    this.notifications.forEach((notification) => {
      notification.isRead = true;
    });

    this.updateUnreadCount();

    // TODO:
    // Call backend endpoint here later
    // to persist all notifications as read.
  }

  // =========================================================
  // Notification Icons
  // =========================================================

  getNotificationIcon(type: string): string {
    switch (type.toLowerCase()) {
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
    const notificationType = type.toLowerCase();

    // Admin
    if (this.isAdmin()) {
      switch (notificationType) {
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
      switch (notificationType) {
        case 'assignment':
          return 'notification-agent-assignment';

        case 'update':
          return 'notification-agent-update';

        case 'comment':
          return 'notification-agent-comment';

        case 'ticket':
          return 'notification-agent';

        default:
          return 'notification-agent';
      }
    }

    // Customer
    if (this.isCustomer()) {
      switch (notificationType) {
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

  async logout(): Promise<void> {
    // Stop SignalR first
    await this.notificationSignalRService.stopConnection();

    // Clear notifications
    this.notifications = [];
    this.unreadCount = 0;

    // Logout
    this.authService.logout();

    // Navigate
    this.router.navigate(['/login']);
  }
}
