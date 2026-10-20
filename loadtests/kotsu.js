// Skeleton only: the normal (10 requests per second) and peak (50) profiles are written in phase 41.
import http from 'k6/http';
import { check } from 'k6';

export const options = {
  vus: 1,
  iterations: 1,
};

const baseUrl = __ENV.BASE_URL || 'http://localhost:5000';

export default function () {
  const response = http.get(`${baseUrl}/`);

  check(response, {
    'the API answers': (r) => r.status !== 0,
  });
}
