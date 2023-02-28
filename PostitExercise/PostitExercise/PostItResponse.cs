namespace PostitExercise
{
    public class PostItResponse
    {
        public List<PostItSingleResponseData> data { get; set; }
    }

    public class PostItSingleResponseData
    {
        public string post_code { get; set; }
    }
}