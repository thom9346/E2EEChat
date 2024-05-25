import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FriendService } from '../services/friend.service';

@Component({
  selector: 'app-verify-friend-request',
  templateUrl: './verify-friend-request.component.html',
  styleUrls: ['./verify-friend-request.component.scss']
})
export class VerifyFriendRequestComponent implements OnInit {

  verificationStatus: string = '';

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private friendService: FriendService
  ) { }

  ngOnInit(): void {
    const requestId = this.route.snapshot.queryParamMap.get('requestId');
    let token = this.route.snapshot.queryParamMap.get('token');

    if (requestId && token) {
      //spaces from queryparams could be tranlated wrongly
      token = token.replace(/ /g, '+');

      const decodedToken = decodeURIComponent(token.trim());
      this.friendService.confirmFriendRequest(requestId, decodedToken).subscribe({
        next: (response) => {
          this.verificationStatus = 'Friend request confirmed successfully.';
        },
        error: (error) => {
          console.error('Failed to confirm friend request:', error);
          this.verificationStatus = 'Failed to confirm friend request. The link might be invalid or expired.';
        }
      });
    } else {
      this.verificationStatus = 'Invalid verification link.';
    }
  }
}
