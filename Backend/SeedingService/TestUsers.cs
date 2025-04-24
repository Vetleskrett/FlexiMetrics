namespace SeedingService;

public class FeideTestUser
{
    public required string Username { get; set; }
    public required string Name { get; set; }
}

public static class TestUsers
{
    public static readonly FeideTestUser[] FEIDE_TEST_TEACHERS =
    [
        new FeideTestUser
        {
            Username = "alf123teacher",
            Name = "Alf Alfsen",
        },
        new FeideTestUser
        {
            Username = "anne123teacher",
            Name = "Anne Istad",
        },
        new FeideTestUser
        {
            Username = "inger456teacher",
            Name = "Inger Karlsen",
        },
        new FeideTestUser
        {
            Username = "jan123teacher",
            Name = "Jan Jansen",
        },
        new FeideTestUser
        {
            Username = "per456teacher",
            Name = "Per Land",
        },
        new FeideTestUser
        {
            Username = "roger123teacher",
            Name = "Roger Mikkelsen",
        }
    ];

    public static readonly FeideTestUser[] FEIDE_TEST_STUDENTS =
    [
        new FeideTestUser
        {
            Username = "alexander123elev",
            Name = "Alexander Hansen",
        },
        new FeideTestUser
        {
            Username = "andreas123elev",
            Name = "Andreas Foss",
        },
        new FeideTestUser
        {
            Username = "emma123elev",
            Name = "Emma Bjørnsen",
        },
        new FeideTestUser
        {
            Username = "ida456elev",
            Name = "Ida Glad",
        },
        new FeideTestUser
        {
            Username = "irene123elev",
            Name = "Irene Vik",
        },
        new FeideTestUser
        {
            Username = "jan_elevvgs",
            Name = "Jan ElevVGS Olsen",
        },
        new FeideTestUser
        {
            Username = "jonas456elev",
            Name = "Jonas Hansen",
        },
        new FeideTestUser
        {
            Username = "julie123elev",
            Name = "Julie Eriksen",
        },
        new FeideTestUser
        {
            Username = "kristian456elev",
            Name = "Kristian Frank",
        },
        new FeideTestUser
        {
            Username = "markus123elev",
            Name = "Markus Andersen",
        },
        new FeideTestUser
        {
            Username = "matias123elev",
            Name = "Matias Duck",
        },
        new FeideTestUser
        {
            Username = "nora123elev",
            Name = "Nora Gundersen",
        },
        new FeideTestUser
        {
            Username = "sara123elev",
            Name = "Sara Christiansen",
        },
        new FeideTestUser
        {
            Username = "thea456elev",
            Name = "Thea Eide",
        },
        new FeideTestUser
        {
            Username = "ulrik123elev",
            Name = "Ulrik Mulvik",
        },
        new FeideTestUser
        {
            Username = "eva_student",
            Name = "Eva Student Åsen",
        },
        new FeideTestUser
        {
            Username = "jakob_student",
            Name = "Jakob Student Åsen",
        },
        new FeideTestUser
        {
            Username = "kari_student",
            Name = "Kari Student Edvardsen",
        },
        new FeideTestUser
        {
            Username = "kristine789student",
            Name = "Kristine Rask",
        },
        new FeideTestUser
        {
            Username = "perra789student",
            Name = "Per Andreas Persen",
        },
        new FeideTestUser
        {
            Username = "silje789student",
            Name = "Silje Olsen",
        },
        new FeideTestUser
        {
            Username = "stian789student",
            Name = "Stian Stil",
        },
        new FeideTestUser
        {
            Username = "thomas789student",
            Name = "Thomas Persen",
        },
        new FeideTestUser
        {
            Username = "alf_elevg",
            Name = "Alf ElevG Christiansen",
        },
        new FeideTestUser
        {
            Username = "anders_elevvgs",
            Name = "Anders ElevVGS Iversen",
        },
        new FeideTestUser
        {
            Username = "anita_elevg",
            Name = "Anita Berge",
        },
        new FeideTestUser
        {
            Username = "ann_elevg",
            Name = "Ann ElevG Berntsen",
        },
        new FeideTestUser
        {
            Username = "asjborn_elevg",
            Name = "Asbjørn ElevG Hansen",
        },
        new FeideTestUser
        {
            Username = "cecilie_elevvgs",
            Name = "Cecilie ElevVGS Ås",
        }
    ];
}
