using System.ComponentModel.DataAnnotations;

namespace Application.DependencyInjection.Options
{
    public record EventStoreOptions
    {
        [Required, Range(3, 100)] 
        public int SnapshotInterval { get; init; }
    }
}