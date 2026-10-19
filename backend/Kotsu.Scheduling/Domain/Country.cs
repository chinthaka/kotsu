namespace Kotsu.Scheduling.Domain;

/// <summary>
/// A country Kotsu serves, with its own qualification stages and exam boards.
/// </summary>
public sealed class Country
{
    private Country(CountryId id, string name, IReadOnlyList<QualificationStage> stages, IReadOnlyList<ExamBoard> boards)
    {
        Id = id;
        Name = name;
        Stages = stages;
        Boards = boards;
    }

    public CountryId Id { get; }

    public string Name { get; }

    public IReadOnlyList<QualificationStage> Stages { get; }

    public IReadOnlyList<ExamBoard> Boards { get; }

    /// <summary>
    /// True when the student picks a board, because the country has more than one.
    /// </summary>
    public bool RequiresBoardChoice => Boards.Count > 1;

    public static Result<Country> Create(
        CountryId id,
        string? name,
        IEnumerable<QualificationStage> stages,
        IEnumerable<ExamBoard> boards)
    {
        var stageList = stages.ToList().AsReadOnly();
        var boardList = boards.ToList().AsReadOnly();

        var errors = Domain.Name.Validate(name);
        if (stageList.Count == 0)
        {
            errors.Add(new("country.no-stages", "A country needs at least one stage."));
        }

        if (boardList.Count == 0)
        {
            errors.Add(new("country.no-boards", "A country needs at least one exam board."));
        }

        if (Collections.HasDuplicates(stageList, s => s.Id))
        {
            errors.Add(new("country.duplicate-stage", "A country can't list the same stage twice."));
        }

        if (Collections.HasDuplicates(boardList, b => b.Id))
        {
            errors.Add(new("country.duplicate-board", "A country can't list the same exam board twice."));
        }

        return Result.From(errors, () => new Country(id, name!, stageList, boardList));
    }
}
