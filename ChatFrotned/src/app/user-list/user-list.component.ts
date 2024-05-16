import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { User } from '../models/User';
import { UserService } from '../services/user.service';
import { AuthService } from '../services/auth.service';
import { EncryptionService } from '../services/encryption.service';

@Component({
  selector: 'app-user-list',
  templateUrl: './user-list.component.html',
  styleUrls: ['./user-list.component.scss']
})
export class UserListComponent {
  users: User[] = [];
  currentUser: any;

  currentUserPublicKey: string;
  currentUserPrivateKey: string;

  @Output() userSelected = new EventEmitter<User>();

  constructor(private userService: UserService, private authService: AuthService, private encryption: EncryptionService) 
  {
    const { publicKey, privateKey } = this.encryption.generateKeyPair();

    this.currentUserPublicKey = publicKey;
    this.currentUserPrivateKey = privateKey;
    localStorage.setItem('privateKey', privateKey);

    //this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit() {
    this.userService.getUsers().subscribe(users => {
      this.users = users;
    });
  }


  startChat(user: User): void {
    console.log(user)
    this.userSelected.emit(user);
    const otherPublicKey = user.publicKey;
    const sharedSecret = this.encryption.computeSharedSecret(this.currentUserPrivateKey, otherPublicKey);
    localStorage.setItem(`sharedSecret_${user.userId}`, sharedSecret);
  }
}
