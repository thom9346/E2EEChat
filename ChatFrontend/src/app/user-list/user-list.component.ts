import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { User } from '../models/User';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent {
  users: User[] = [];
  currentUser: any;
  @Output() userSelected = new EventEmitter<User>();

  constructor(
    private userService: UserService,
    private authService: AuthService) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.userService.getUsers().subscribe({
      next: (data) => {
        const currentUser = this.authService.getCurrentUser();
        this.users = data.filter(user => user.userId !== currentUser.id);
      },
      error: (error) => console.error('Failed to get users:', error)
    });
  }

  selectUser(user: User) {
    this.userSelected.emit(user);
  }
}
