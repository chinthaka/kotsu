import { cleanup, render, screen } from '@testing-library/react'
import { afterEach, describe, expect, it } from 'vitest'
import App from './App.tsx'

afterEach(cleanup)

describe('App', () => {
  it('shows the Kotsu heading', () => {
    // Act
    render(<App />)

    // Assert
    expect(screen.getByRole('heading', { name: 'Kotsu' })).toBeDefined()
  })
})
