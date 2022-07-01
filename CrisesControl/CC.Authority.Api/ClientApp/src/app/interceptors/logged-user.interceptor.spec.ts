import { TestBed } from '@angular/core/testing';

import { LoggedUserInterceptor } from './logged-user.interceptor';

describe('LoggedUserInterceptor', () => {
  beforeEach(() => TestBed.configureTestingModule({
    providers: [
      LoggedUserInterceptor
      ]
  }));

  it('should be created', () => {
    const interceptor: LoggedUserInterceptor = TestBed.inject(LoggedUserInterceptor);
    expect(interceptor).toBeTruthy();
  });
});
