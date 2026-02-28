// Stores the location of a token in the source code
public readonly struct Location {
    public readonly int line;
    public readonly int position;

    public Location(int line, int position) {
        this.line = line;
        this.position = position;
    }
}