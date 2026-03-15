public class Getc : Stmt {
    private Assignable target; 

    public Getc(Assignable target) {
        this.target = target;
    }

    public override Value exec(Scope _) {
        int input = Console.Read(); 
        target.set(Value.number(input), _); 
        return Value.number(input);
    }

    public override string ToString() {
        return "GETC " + target;
    }
}