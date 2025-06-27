using dm.PulseShift.Domain.Entities;
using dm.PulseShift.Domain.Interfaces.Repositories;
using dm.PulseShift.Infra.Data.Context;
using dm.PulseShift.Infra.Data.Repositories.Base;

namespace dm.PulseShift.Infra.Data.Repositories;
public class SprintReportRepository(PulseShiftDbContext db)
    : BaseRepository<SprintReport>(db), ISprintReportRepository
{ }