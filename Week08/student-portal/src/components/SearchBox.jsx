// Task 8.3 — controlled search input; filters are applied by the parent via onChange
export default function SearchBox({ value, onChange, matchCount }) {
  return (
    <div style={{ display: 'flex', alignItems: 'center', gap: '1rem', marginBottom: '1rem' }}>
      <input
        type="text"
        placeholder="Search by name…"
        value={value}
        onChange={e => onChange(e.target.value)}
        aria-label="search"
        style={{ width: 260, marginBottom: 0 }}
      />
      <span data-testid="match-count">{matchCount} student{matchCount !== 1 ? 's' : ''} found</span>
    </div>
  );
}
