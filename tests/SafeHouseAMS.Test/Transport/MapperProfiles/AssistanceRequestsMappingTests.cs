using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using FluentAssertions;
using FsCheck;
using FsCheck.Xunit;
using SafeHouseAMS.BizLayer.AssistanceRequests;
using SafeHouseAMS.BizLayer.ExploitationEpisodes;
using SafeHouseAMS.Transport.MapperProfiles;

namespace SafeHouseAMS.Test.Transport.MapperProfiles;

public class AssistanceRequestsMappingTests
{
    private Mapper BuildMapper()
    {
        var cfg = new MapperConfiguration(c => c.AddMaps(typeof(AssistanceRequestsMappingProfile).Assembly));
        return new(cfg);
    }

    [Property]
    public void RoundTripMapping_ReturnsSameAssistanceAct()
    {
        Arb.Register<DateTimeGenerators>();
        Arb.Register<NotNullStringsGenerators>();

        var mapper = BuildMapper();

        Prop.ForAll<AssistanceAct>(src =>
        {
            var dto = mapper.Map<SafeHouseAMS.Transport.Protos.Models.AssistanceRequests.AssistanceAct>(src);
            var result = mapper.Map<AssistanceAct>(dto);

            result.Should()
                .BeEquivalentTo(src, opts => opts.IncludingProperties());
        }).QuickCheckThrowOnFailure();
    }

    [Property]
    public void RoundTripMapping_ReturnsSameAssistanceRequest()
    {
        Arb.Register<DateTimeGenerators>();
        Arb.Register<NotNullStringsGenerators>();
        Arb.Register<AssistanceActListGenerators>();

        var mapper = BuildMapper();

        Prop.ForAll<AssistanceRequest>(src =>
        {
            var dto = mapper.Map<SafeHouseAMS.Transport.Protos.Models.AssistanceRequests.AssistanceRequest>(src);
            var result = mapper.Map<AssistanceRequest>(dto);

            result.Should()
                .BeEquivalentTo(src, opts => opts.IncludingProperties());
        }).QuickCheckThrowOnFailure();
    }
}

public class AssistanceActListGenerators
{
    public static Arbitrary<IReadOnlyCollection<AssistanceAct>> ActsList =>
        Arb.From(Arb.From<AssistanceAct>().Generator.ListOf().Select(x => x.ToList() as IReadOnlyCollection<AssistanceAct>));
}
