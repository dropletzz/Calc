using System.Buffers;

public readonly struct Value {
    public enum Kind {
        Number,
        Array
    }

    public readonly Kind kind;
    public readonly double num;
    public readonly double[] arr;
    public readonly int capacity;

    private Value(Kind kind, double num) {
        this.kind = kind;
        this.num = num;
        this.arr = Array.Empty<double>();
        this.capacity = 0;
    }

    private Value(Kind kind, double[] arr, int capacity) {
        this.kind = kind;
        this.num = 0;
        this.arr = arr;
        this.capacity = capacity;
    }

    public static Value number(double num) {
        return new Value(Kind.Number, num);
    }

    public static Value array(int capacity) {
        double[] arr = ArrayPool<double>.Shared.Rent(capacity); 
        return new Value(Kind.Array, arr, capacity);
    }

    public void free() {
        if (this.kind == Kind.Array)
            ArrayPool<double>.Shared.Return(arr);
    }

    public bool isCopy() {
        return this.kind == Kind.Number;
    }

    public Value clone() {
        if (this.kind == Kind.Array) {
            Value clone = Value.array(this.capacity);
            Array.Copy(this.arr, clone.arr, this.capacity);
            return clone;
        }
        return Value.number(num);
    }

    public override string ToString() {
        if (kind == Kind.Number) return num.ToString();
        if (arr != null && kind == Kind.Array) {
            int i = 0;
            string result = "[ ";
            while (i < capacity-1) result += arr[i++] + ", ";
            result += arr[i] + " ]"; 
            return result;
        }
        return kind.ToString();
    }
}