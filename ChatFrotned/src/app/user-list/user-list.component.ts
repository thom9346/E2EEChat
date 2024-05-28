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
        console.log("currentUser")
        console.log(currentUser)
        this.users = data.filter(user => user.userId !== currentUser.id);
        console.log(this.users)
      },
      error: (error) => console.error('Failed to get users:', error)
    });
  }

  selectUser(user: User) {
    console.log("clicked");
    console.log(user);
    this.userSelected.emit(user);
  }
}
