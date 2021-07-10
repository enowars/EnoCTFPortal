namespace EnoLandingPageBackend.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using EnoCore.Models;
    using EnoCore.Scoreboard;

    public class OverrideScoreboard
    {
        public OverrideScoreboard(long currentRound, string? startTimestamp, string? endTimestamp, string? dnsSuffix, ScoreboardService[] services, OverrideScoreboardTeam[] teams)
        {
            this.CurrentRound = currentRound;
            this.StartTimestamp = startTimestamp;
            this.EndTimestamp = endTimestamp;
            this.DnsSuffix = dnsSuffix;
            this.Services = services;
            this.Teams = teams;
        }
        [Required]
        public long CurrentRound { get; set; }
        public string? StartTimestamp { get; set; }
        public string? EndTimestamp { get; set; }
        public string? DnsSuffix { get; set; }
        [Required]
        public ScoreboardService[] Services { get; set; }
        [Required]
        public OverrideScoreboardTeam[] Teams { get; set; }

    }

    public class OverrideScoreboardTeam
    {
        public OverrideScoreboardTeam(string teamName, long teamId, string? logoUrl, string? countryCode, double totalScore, double attackScore, double defenseScore, double serviceLevelAgreementScore, OverrideScoreboardTeamServiceDetails[] serviceDetails, double totalScoreDelta, double attackScoreDelta, double defenseScoreDelta, double serviceLevelAgreementScoreDelta)
        {
            this.TeamName = teamName;
            this.TeamId = teamId;
            this.LogoUrl = logoUrl;
            this.CountryCode = countryCode;
            this.TotalScore = totalScore;
            this.AttackScore = attackScore;
            this.DefenseScore = defenseScore;
            this.ServiceLevelAgreementScore = serviceLevelAgreementScore;
            this.ServiceDetails = serviceDetails;
            this.totalScoreDelta = totalScoreDelta;
            this.attackScoreDelta = attackScoreDelta;
            this.defenseScoreDelta = defenseScoreDelta;
            this.serviceLevelAgreementScoreDelta = serviceLevelAgreementScoreDelta;
        }
        [Required]
        public string TeamName { get; init; }
        [Required]
        public long TeamId { get; init; }
        public string? LogoUrl { get; init; }
        public string? CountryCode { get; init; }
        [Required]
        public double TotalScore { get; init; }
        [Required]
        public double AttackScore { get; init; }
        [Required]
        public double DefenseScore { get; init; }
        [Required]
        public double ServiceLevelAgreementScore { get; init; }
        public OverrideScoreboardTeamServiceDetails[] ServiceDetails { get; set; }
        public double totalScoreDelta { get; set; }
        public double attackScoreDelta { get; set; }
        public double defenseScoreDelta { get; set; }
        public double serviceLevelAgreementScoreDelta { get; set; }

    }

    public class OverrideScoreboardTeamServiceDetails : ScoreboardTeamServiceDetails
    {

        public OverrideScoreboardTeamServiceDetails(long serviceId, double attackScore, double defenseScore, double serviceLevelAgreementScore, ServiceStatus serviceStatus, string? message, double attackScoreDelta, double defenseScoreDelta, double serviceLevelAgreementScoreDelta)
        : base(serviceId, attackScore, defenseScore, serviceLevelAgreementScore, serviceStatus, message)
        {
            this.attackScoreDelta = attackScoreDelta;
            this.defenseScoreDelta = defenseScoreDelta;
            this.serviceLevelAgreementScoreDelta = serviceLevelAgreementScoreDelta;
        }

        public double attackScoreDelta { get; set; }
        public double defenseScoreDelta { get; set; }
        public double serviceLevelAgreementScoreDelta { get; set; }
    }
}
