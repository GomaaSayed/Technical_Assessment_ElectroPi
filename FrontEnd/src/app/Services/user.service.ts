import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ConfigService } from '../shared/services/config-service';
import { CreateUserDTO } from '../DTOs/CreateUserDTO';
import { UpdateUserDTO } from '../DTOs/UpdateUserDTO';

@Injectable({
  providedIn: 'root',
})
export class UserService {
  private config: any;

  constructor(
    private http: HttpClient,
    private configService: ConfigService,
  ) {
    this.configService.getConfig().subscribe((config) => {
      this.config = config;
    });
  }

  getAllUsers(): Observable<any[]> {
    return this.http.get<any[]>(`${this.config.baseUrl}User`);
  }
  createUser(user: CreateUserDTO): Observable<any> {
    return this.http.post<any>(`${this.config.baseUrl}User`, user);
  }
  getUserById(userId: string): Observable<any> {
    return this.http.get<any>(`${this.config.baseUrl}User/${userId}`);
  }
  updateUser(userId: string, user: UpdateUserDTO): Observable<any> {
    return this.http.put<any>(`${this.config.baseUrl}User/${userId}`, user);
  }

  deleteUser(userId: string): Observable<any> {
    return this.http.delete<any>(`${this.config.baseUrl}User/${userId}`);
  }

  getAgents(): Observable<any[]> {
    return this.http.get<any[]>(`${this.config.baseUrl}User/agents`);
  }
}
