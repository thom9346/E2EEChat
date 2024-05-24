import { ComponentFixture, TestBed } from '@angular/core/testing';

import { VerifyFriendRequestComponent } from './verify-friend-request.component';

describe('VerifyFriendRequestComponent', () => {
  let component: VerifyFriendRequestComponent;
  let fixture: ComponentFixture<VerifyFriendRequestComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ VerifyFriendRequestComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(VerifyFriendRequestComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
