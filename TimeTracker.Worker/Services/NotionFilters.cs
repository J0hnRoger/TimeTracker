namespace TimeTracker.Worker.Services;
public class Filter
{
    public List<OrFilter> Or { get; set; }
}

public class OrFilter
{
    public PropertyFilter Property { get; set; }
}

public class PropertyFilter
{
    public string Property { get; set; }
    public CheckboxFilter Checkbox { get; set; }
    public NumberFilter Number { get; set; }
}

public class CheckboxFilter
{
    public bool Equals { get; set; }
}

public class NumberFilter
{
    public int GreaterThanOrEqualTo { get; set; }
}

public class Sort
{
    public string Property { get; set; }
    public string Direction { get; set; }
}

public class QueryRequest
{
    public Filter Filter { get; set; }
    public List<Sort> Sorts { get; set; }
}
