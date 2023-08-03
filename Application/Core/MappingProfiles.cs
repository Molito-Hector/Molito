using Application.Activities;
using Application.Comments;
using Application.Profiles;
using Domain;

namespace Application.Core
{
    public class MappingProfiles : AutoMapper.Profile
    {
        public MappingProfiles()
        {
            string currentUsername = null;

            CreateMap<Activity, Activity>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
            CreateMap<RuleProject, RuleProjectDto>()
                .ForMember(d => d.Properties, o => o.MapFrom(s => s.Properties.Where(c => c.ParentPropertyId == null)));
            CreateMap<RuleProjectDto, RuleProject>();
            CreateMap<RuleProject, RuleProjectListDto>();
            CreateMap<Rule, Rule>();
            CreateMap<Rule, RuleListDto>();
            CreateMap<Rule, RuleDto>()
                .ForMember(d => d.Conditions, o => o.MapFrom(s => s.Conditions.Where(c => c.ParentConditionId == null)));
            CreateMap<Rule, RuleWithProjectDto>()
                .ForMember(d => d.Conditions, o => o.MapFrom(s => s.Conditions.Where(c => c.ParentConditionId == null)));
            CreateMap<DecisionTable, DecisionTableDto>()
                .ForMember(d => d.Conditions, o => o.MapFrom(s => s.Conditions.Where(c => c.ParentConditionId == null)));
            CreateMap<DecisionTable, DTWithProjectDto>()
                .ForMember(d => d.Conditions, o => o.MapFrom(s => s.Conditions.Where(c => c.ParentConditionId == null)));
            CreateMap<DecisionTable, DecisionTableListDto>();
            CreateMap<DecisionTableDto, DecisionTable>();
            CreateMap<DecisionRow, DecisionRowDto>();
            CreateMap<DecisionRowDto, DecisionRow>();
            CreateMap<RuleWithProjectDto, Rule>();
            CreateMap<Domain.Action, ActionDto>()
                 .ForMember(d => d.Id, o => o.MapFrom(s => s.Id))
                 .ForMember(d => d.ModificationType, o => o.MapFrom(s => s.ModificationType.ToString()))
                 .ForMember(d => d.Name, o => o.MapFrom(s => s.Name));
            CreateMap<ActionDto, Domain.Action>();
            CreateMap<Condition, ConditionDto>();
            CreateMap<ConditionDto, Condition>();
            CreateMap<Condition, SubConditionDto>();
            CreateMap<SubConditionDto, Condition>();
            CreateMap<RuleProperty, RulePropertyDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
            CreateMap<RuleProperty, SubpropertyDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()));
            CreateMap<RulePropertyDto, RuleProperty>();
            CreateMap<SubpropertyDto, RuleProperty>();
            CreateMap<ActivityAttendee, AttendeeDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.AppUser.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.AppUser.Followings.Count))
                .ForMember(d => d.Following, o => o.MapFrom(s => s.AppUser.Followers.Any(x => x.Observer.UserName == currentUsername)));
            CreateMap<RuleProjectMember, RuleProjectMemberDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.AppUser.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.AppUser.UserName))
                .ForMember(d => d.Bio, o => o.MapFrom(s => s.AppUser.Bio))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<AppUser, Profiles.Profile>()
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(d => d.FollowersCount, o => o.MapFrom(s => s.Followers.Count))
                .ForMember(d => d.FollowingCount, o => o.MapFrom(s => s.Followings.Count))
                .ForMember(d => d.Following, o => o.MapFrom(s => s.Followers.Any(x => x.Observer.UserName == currentUsername)));
            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.DisplayName, o => o.MapFrom(s => s.Author.DisplayName))
                .ForMember(d => d.Username, o => o.MapFrom(s => s.Author.UserName))
                .ForMember(d => d.Image, o => o.MapFrom(s => s.Author.Photos.FirstOrDefault(x => x.IsMain).Url));
            CreateMap<ActivityAttendee, UserActivityDto>()
                .ForMember(d => d.Id, o => o.MapFrom(s => s.Activity.Id))
                .ForMember(d => d.Date, o => o.MapFrom(s => s.Activity.Date))
                .ForMember(d => d.Title, o => o.MapFrom(s => s.Activity.Title))
                .ForMember(d => d.Category, o => o.MapFrom(s => s.Activity.Category))
                .ForMember(d => d.HostUsername, o => o.MapFrom(s => s.Activity.Attendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));
        }
    }
}