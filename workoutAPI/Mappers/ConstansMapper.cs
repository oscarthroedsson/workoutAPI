public class ConstantsMapper
{
    public static Dictionary<string, Dictionary<string, object>> MapFromTable(List<ConstantsDTO> table)
    {
        return table
            .GroupBy(x => x.TableName.ToLower())
            .ToDictionary(
                group => group.Key,
                group =>
                {
                    var constantGroup = new Dictionary<string, object>();

                    foreach (var item in group)
                    {
                        constantGroup[item.Code] = item.Code;
                    }

                    constantGroup["arr"] = group.Select(item => item.Code).ToList();

                    return constantGroup;
                }
            );
    }
}