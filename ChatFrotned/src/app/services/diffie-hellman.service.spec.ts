import { TestBed } from '@angular/core/testing';

import { DiffieHellmanService } from './diffie-hellman.service';

describe('DiffieHellmanService', () => {
  let service: DiffieHellmanService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(DiffieHellmanService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
