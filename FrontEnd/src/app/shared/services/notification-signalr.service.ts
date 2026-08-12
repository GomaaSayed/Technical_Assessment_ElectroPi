import { Injectable } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  LogLevel,
} from '@microsoft/signalr';
import { Subject, firstValueFrom } from 'rxjs';

import { ConfigService } from './config-service';

export interface NotificationItem {
  id: string;
  title: string;
  message: string;
  type: string;
  referenceId?: string;
  isRead: boolean;
  createdAt: Date;
}

@Injectable({
  providedIn: 'root',
})
export class NotificationSignalRService {
  private hubConnection?: HubConnection;

  private readonly notificationSubject = new Subject<NotificationItem>();

  readonly notification$ = this.notificationSubject.asObservable();

  constructor(private configService: ConfigService) {}

  async startConnection(): Promise<void> {
    // Already connected / initialized
    if (this.hubConnection) {
      return;
    }

    const config = await firstValueFrom(this.configService.getConfig());

    if (!config.notificationHubUrl) {
      throw new Error('Notification Hub URL is not configured.');
    }

    this.hubConnection = new HubConnectionBuilder()
      .withUrl(config.notificationHubUrl, {
        accessTokenFactory: () => localStorage.getItem('token') ?? '',
      })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Information)
      .build();

    this.hubConnection.on(
      'ReceiveNotification',
      (notification: NotificationItem) => {
        console.log('Received notification:', notification);

        notification.createdAt = new Date(notification.createdAt);

        this.notificationSubject.next(notification);
      },
    );

    try {
      await this.hubConnection.start();

      console.log('Notification Hub connected');
    } catch (error) {
      console.error('Notification Hub connection failed:', error);

      this.hubConnection = undefined;

      throw error;
    }
  }

  async stopConnection(): Promise<void> {
    if (!this.hubConnection) {
      return;
    }

    try {
      await this.hubConnection.stop();

      console.log('Notification Hub disconnected');
    } finally {
      this.hubConnection = undefined;
    }
  }
}
