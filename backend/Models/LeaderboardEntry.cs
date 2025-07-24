namespace WebApplication1.Models//Diğer sınıflar using WebApplication1.Models; diyerek bu modeli kullanabilir.
{//Bu dosya, frontend’e veya API’ye gönderilecek veri modelini temsil eder.


    public class LeaderboardEntry
    {//required: C# 11 özelliği. Bu özellik sayesinde nesne oluşturulurken bu alanın mutlaka set edilmesi gerekir.
        public required string Username { get; set; }
        public double Score { get; set; }//Redis SortedSet’lerde skorlar hep double (ondalıklı) olarak saklanır.
        public int Rank { get; set; }
    }/*“LeaderboardEntry.cs, Redis’ten çekilen verilerin frontend’e JSON olarak gönderilmeden önce temsil edildiği modeldir. Kullanıcı adı, puan ve sıralamayı içerir.
     Bu sayede her bir kullanıcı satırı düzgün yapılandırılmış şekilde React’a iletilir.”*/
}