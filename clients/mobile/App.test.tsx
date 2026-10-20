import { render, screen } from '@testing-library/react-native';
import App from './App';

describe('App', () => {
  it('shows the Kotsu heading', async () => {
    // Act
    await render(<App />);

    // Assert
    expect(screen.getByRole('header', { name: 'Kotsu' })).toBeTruthy();
  });
});
