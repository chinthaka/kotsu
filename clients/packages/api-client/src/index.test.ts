import { describe, expect, it } from 'vitest';

describe('api-client', () => {
  it('loads with nothing generated yet', async () => {
    // Act
    const client = await import('./index');

    // Assert
    expect(Object.keys(client)).toEqual([]);
  });
});
