import { render, screen, fireEvent } from '@testing-library/react';
import { useState } from 'react';
import SearchBox from '../components/SearchBox';

// Wrapper that wires the search box to live-filter a list (Task 8.3)
function SearchWrapper({ students }) {
  const [q, setQ] = useState('');
  const filtered = students.filter(s =>
    s.name.toLowerCase().includes(q.toLowerCase())
  );
  return (
    <>
      <SearchBox value={q} onChange={setQ} matchCount={filtered.length} />
      <ul>
        {filtered.map(s => <li key={s.id}>{s.name}</li>)}
      </ul>
    </>
  );
}

const STUDENTS = [
  { id: 1, name: 'Elakkia' },
  { id: 2, name: 'Ramya'   },
  { id: 3, name: 'Arjun'   },
];

describe('SearchBox', () => {
  test('shows all students when query is empty', () => {
    render(<SearchWrapper students={STUDENTS} />);
    expect(screen.getByText('Elakkia')).toBeInTheDocument();
    expect(screen.getByText('Ramya')).toBeInTheDocument();
    expect(screen.getByText('Arjun')).toBeInTheDocument();
  });

  test('filters list as user types (Task 8.3)', () => {
    render(<SearchWrapper students={STUDENTS} />);
    fireEvent.change(screen.getByRole('textbox'), { target: { value: 'ela' } });
    expect(screen.getByText('Elakkia')).toBeInTheDocument();
    expect(screen.queryByText('Ramya')).not.toBeInTheDocument();
    expect(screen.queryByText('Arjun')).not.toBeInTheDocument();
  });

  test('is case-insensitive', () => {
    render(<SearchWrapper students={STUDENTS} />);
    fireEvent.change(screen.getByRole('textbox'), { target: { value: 'ELA' } });
    expect(screen.getByText('Elakkia')).toBeInTheDocument();
  });

  test('shows match counter updating live', () => {
    render(<SearchWrapper students={STUDENTS} />);
    expect(screen.getByTestId('match-count')).toHaveTextContent('3 students found');
    fireEvent.change(screen.getByRole('textbox'), { target: { value: 'ram' } });
    expect(screen.getByTestId('match-count')).toHaveTextContent('1 student found');
  });

  test('shows zero matches for no-result query', () => {
    render(<SearchWrapper students={STUDENTS} />);
    fireEvent.change(screen.getByRole('textbox'), { target: { value: 'zzz' } });
    expect(screen.getByTestId('match-count')).toHaveTextContent('0 students found');
  });
});
