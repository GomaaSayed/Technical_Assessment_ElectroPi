import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, map, switchMap, take } from 'rxjs';

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
export class NotificationService {
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

  getMyNotifications(): Observable<NotificationItem[]> {
    return this.getBaseUrl().pipe(
      switchMap((baseUrl) =>
        this.http.get<NotificationItem[]>(`${baseUrl}Notification`),
      ),
      map((notifications) =>
        notifications.map((notification) => ({
          ...notification,
          createdAt: new Date(notification.createdAt),
        })),
      ),
    );
  }
}
