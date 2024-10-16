using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TimeTracker.Worker.Services;

public class NotionApi
{
    private readonly HttpClient _httpClient;

    public NotionApi(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<NotionResponse> GetTaskFromDatabase(string databaseId)
    {
        // Construire la requête
        var request = new {filter = new {property = "Type", select = new {equals = "Task"}}};

        var jsonRequest = JsonSerializer.Serialize(request,
            new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase});

        // Créer le message HTTP
        var httpRequestMessage =
            new HttpRequestMessage(HttpMethod.Post, $"/v1/databases/{databaseId}/query")
            {
                Content = new StringContent(jsonRequest, Encoding.UTF8, "application/json")
            };

        // Envoyer la requête
        var response = await _httpClient.SendAsync(httpRequestMessage);

        if (!response.IsSuccessStatusCode)
        {
            // Gérer les erreurs de requête
            throw new Exception($"Erreur de l'API Notion : {response.StatusCode}");
        }

        // Lire et désérialiser la réponse JSON
        var jsonResponse = await response.Content.ReadAsStringAsync();
        var tasks = JsonSerializer.Deserialize<NotionResponse>(jsonResponse, new JsonSerializerOptions()
        {
            PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
        });
        return tasks ?? new NotionResponse();
    }
}

public class NotionResponse
{
    public string Object { get; set; }
    public List<NotionPage> Results { get; set; }
    public string NextCursor { get; set; }
    public bool HasMore { get; set; }
    public string Type { get; set; }
}

public class NotionPage
{
    public string Object { get; set; }
    public string Id { get; set; }
    public DateTime CreatedTime { get; set; }
    public DateTime LastEditedTime { get; set; }
    public NotionUser CreatedBy { get; set; }
    public NotionUser LastEditedBy { get; set; }
    public NotionCover Cover { get; set; }
    public NotionIcon Icon { get; set; }
    public NotionParent Parent { get; set; }
    public bool Archived { get; set; }
    public NotionProperties Properties { get; set; }
    public string Url { get; set; }
}

public class NotionUser
{
    public string Object { get; set; }
    public string Id { get; set; }
}

public class NotionIcon
{
    public string Type { get; set; }
    public string Emoji { get; set; }
}

public class NotionCover
{
    public string Type { get; set; }
    public NotionExternal External { get; set; }
}

public class NotionExternal
{
    public string Url { get; set; }
}

public class NotionParent
{
    public string Type { get; set; }
    public string DatabaseId { get; set; }
}

public class NotionProperties
{
    [JsonPropertyName("Name")]
    public NotionTitleProperty Name { get; set; }
    
    [JsonPropertyName("Area")]
    public NotionSelectProperty Area { get; set; }
    
    [JsonPropertyName("Type")]
    public NotionSelectProperty Type { get; set; }
    
    [JsonPropertyName("DueDate")]
    public NotionDateProperty DueDate { get; set; }
    
    [JsonPropertyName("Description")]
    public NotionRichTextProperty Description { get; set; }
    
    [JsonPropertyName("Estimations (en min) #")]
    public NotionNumberProperty Estimations { get; set; }
    
    [JsonPropertyName("Duration (en minutes) #")]
    public NotionNumberProperty Duration { get; set; }
    
    [JsonPropertyName("Status")]
    public NotionSelectProperty Status { get; set; }
    
    [JsonPropertyName("Difficulty")]
    public NotionSelectProperty Difficulty { get; set; }
}

public class NotionSelectProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public NotionSelect Select { get; set; }
}

public class NotionSelect
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}

public class NotionMultiSelectProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public List<NotionMultiSelect> MultiSelect { get; set; }
}

public class NotionMultiSelect
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Color { get; set; }
}

public class NotionNumberProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public double? Number { get; set; }
}

public class NotionPeopleProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public List<NotionPerson> People { get; set; }
}

public class NotionPerson
{
    public string Object { get; set; }
    public string Id { get; set; }
    public string Name { get; set; }
    public string AvatarUrl { get; set; }
    public NotionPersonDetail Person { get; set; }
}

public class NotionPersonDetail
{
    public string Email { get; set; }
}

public class NotionDateProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public NotionDate Date { get; set; }
}

public class NotionDate
{
    public string Start { get; set; }
    public string End { get; set; }
    public string TimeZone { get; set; }
}

public class NotionFormulaProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public NotionFormula Formula { get; set; }
}

public class NotionFormula
{
    public string Type { get; set; }
    public double? Number { get; set; }
}

public class NotionRelationProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public List<NotionRelation> Relation { get; set; }
    public bool HasMore { get; set; }
}

public class NotionRelation
{
    public string Id { get; set; }
}

public class NotionRichTextProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public List<NotionRichText> RichText { get; set; }
}

public class NotionRichText
{
    public string Type { get; set; }
    public NotionText Text { get; set; }
    public NotionAnnotations Annotations { get; set; }
    public string PlainText { get; set; }
}

public class NotionText
{
    public string Content { get; set; }
    public string Link { get; set; }
}

public class NotionAnnotations
{
    public bool Bold { get; set; }
    public bool Italic { get; set; }
    public bool Strikethrough { get; set; }
    public bool Underline { get; set; }
    public bool Code { get; set; }
    public string Color { get; set; }
}

public class NotionCheckboxProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public bool Checkbox { get; set; }
}

public class NotionRollupProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public NotionRollup Rollup { get; set; }
}

public class NotionRollup
{
    public string Type { get; set; }
    public double? Number { get; set; }
    public string Function { get; set; }
}

public class NotionUrlProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public string Url { get; set; }
}

public class NotionTitleProperty
{
    public string Id { get; set; }
    public string Type { get; set; }
    public List<NotionTitle> Title { get; set; }
}

public class NotionTitle
{
    public string Type { get; set; }
    public NotionText Text { get; set; }
    public string PlainText { get; set; }
}
