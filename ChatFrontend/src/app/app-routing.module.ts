import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ChatWindowComponent } from './chat-window/chat-window.component';
import { AuthGuard } from './guards/auth.guard';
import { RegisterComponent } from './register/register.component';
import { NoAuthGuard } from './guards/noauth.guard';
import { VerifyFriendRequestComponent } from './verify-friend-request/verify-friend-request.component';

const routes: Routes = [
  { path: 'login', component: LoginComponent, canActivate: [NoAuthGuard]  },
  { path: 'register', component: RegisterComponent, canActivate: [NoAuthGuard]  },
  { path: 'chat', component: ChatWindowComponent, canActivate: [AuthGuard] },
  { path: '', redirectTo: '/login', pathMatch: 'full' },
  { path: 'verify-friend-request', component: VerifyFriendRequestComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
