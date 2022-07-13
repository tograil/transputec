import { TestBed } from '@angular/core/testing';

import { UserLoggedInGuard } from './user-logged-in.guard';

describe('UserLoggedInGuard', () => {
  let guard: UserLoggedInGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(UserLoggedInGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
