import { useState, useEffect, useCallback } from "react";

const apiUrl = process.env.REACT_APP_API_URL || "http://localhost:5259";
const courses = ["english", "genel-yetenek", "mulakat"];

function App() {
  const [data, setData] = useState([]);
  const [selectedCourse, setSelectedCourse] = useState("english");
  const [newUsername, setNewUsername] = useState("");
  const [newScore, setNewScore] = useState("");
  const [searchTerm, setSearchTerm] = useState("");

  const fetchData = useCallback(() => {
    fetch(`${apiUrl}/leaderboard/${selectedCourse}`)
      .then((res) => res.json())
      .then(setData)
      .catch(console.error);
  }, [selectedCourse]);

  useEffect(() => {
    fetchData();
    const interval = setInterval(fetchData, 5000);
    return () => clearInterval(interval);
  }, [fetchData]);

  const handleSubmit = (e) => {
    e.preventDefault();
    if (!newUsername || !newScore) return;

    fetch(`${apiUrl}/score/${selectedCourse}/${newUsername}/${newScore}`, {
      method: "POST",
    })
      .then((res) => res.text())
      .then((msg) => {
        alert(msg);
        setNewUsername("");
        setNewScore("");
        fetchData();
      })
      .catch((err) => alert("Hata: " + err.message));
  };

  const filteredData = data.filter((entry) =>
    entry.username.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ display: "flex", padding: "2rem", background: "#f8fafc" }}>
      <div style={{ flex: 1, maxWidth: "900px", margin: "0 auto" }}>
        {/* Başlık */}
        <div style={{ background: "#ffffff", padding: "2rem", borderRadius: "16px", boxShadow: "0 1px 3px rgba(0,0,0,0.1)", marginBottom: "2rem", border: "1px solid #e2e8f0" }}>
          <h1 style={{ fontSize: "2.5rem", color: "#1e40af", fontWeight: "700", marginBottom: "0.5rem" }}>
            🏆 Leaderboard
          </h1>
          <p style={{ color: "#64748b", margin: 0, fontSize: "1.1rem" }}>
            Performans Takip Sistemi
          </p>
        </div>

        {/* Birinci Kartı */}
        {data.length > 0 && (
          <div style={{ background: "linear-gradient(135deg, #f59e0b 0%, #d97706 100%)", color: "white", padding: "2rem", borderRadius: "16px", textAlign: "center", marginBottom: "2rem", fontWeight: "bold", fontSize: "1.25rem" }}>
            <div style={{ fontSize: "2rem", marginBottom: "0.5rem" }}>👑</div>
            <div style={{ fontSize: "1.2rem", marginBottom: "0.25rem" }}>Birinci</div>
            <div style={{ fontSize: "1.6rem" }}>{data[0].username}</div>
            <div style={{ fontSize: "1rem", opacity: 0.9 }}>{data[0].score} puan</div>
          </div>
        )}

        {/* Skor Gönderme */}
        <div style={{ background: "#ffffff", padding: "2rem", borderRadius: "16px", marginBottom: "2rem", border: "1px solid #e2e8f0" }}>
          <h2 style={{ marginBottom: "1rem", color: "#1e293b" }}>Skor Gönder</h2>
          <form onSubmit={handleSubmit} style={{ display: "flex", gap: "1rem", flexWrap: "wrap" }}>
            <input
              type="text"
              placeholder="Kullanıcı adı"
              value={newUsername}
              onChange={(e) => setNewUsername(e.target.value)}
              style={{ flex: 1, padding: "0.75rem", borderRadius: "8px", border: "1px solid #cbd5e1" }}
            />
            <input
              type="number"
              placeholder="Skor"
              value={newScore}
              onChange={(e) => setNewScore(e.target.value)}
              style={{ width: "100px", padding: "0.75rem", borderRadius: "8px", border: "1px solid #cbd5e1" }}
            />
            <button type="submit" style={{ background: "linear-gradient(135deg, #3b82f6 0%, #1d4ed8 100%)", color: "white", padding: "0.75rem 1.5rem", borderRadius: "8px", border: "none", fontWeight: "600", cursor: "pointer" }}>
              Gönder
            </button>
          </form>
        </div>

        {/* Arama ve Tablo */}
        <div style={{ background: "#ffffff", padding: "2rem", borderRadius: "16px", border: "1px solid #e2e8f0" }}>
          <input
            type="text"
            placeholder="🔍 Kullanıcı adı ara..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            style={{ width: "100%", padding: "0.75rem", fontSize: "1rem", borderRadius: "10px", border: "2px solid #e2e8f0", backgroundColor: "#f8fafc", marginBottom: "1rem" }}
          />

          <table style={{ width: "100%", borderCollapse: "collapse" }}>
            <thead>
              <tr style={{ backgroundColor: "#f1f5f9", textAlign: "left" }}>
                <th style={{ padding: "0.75rem" }}>Sıra</th>
                <th style={{ padding: "0.75rem" }}>Kullanıcı</th>
                <th style={{ padding: "0.75rem" }}>Skor</th>
              </tr>
            </thead>
            <tbody>
              {filteredData.map((entry, index) => (
                <tr key={index} style={{ borderTop: "1px solid #e2e8f0" }}>
                  <td style={{ padding: "0.75rem" }}>{index + 1}</td>
                  <td style={{ padding: "0.75rem" }}>{entry.username}</td>
                  <td style={{ padding: "0.75rem" }}>{entry.score}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      {/* Kategori Sidebar */}
      <aside style={{ width: "220px", marginLeft: "2rem", position: "sticky", top: "2rem", height: "fit-content" }}>
        <div style={{ backgroundColor: "#ffffff", padding: "1.5rem", borderRadius: "16px", boxShadow: "0 1px 3px rgba(0,0,0,0.1)", border: "1px solid #e2e8f0" }}>
          <h3 style={{ color: "#1e293b", marginBottom: "1rem", fontSize: "1.2rem", fontWeight: "600" }}>📚 Kategori</h3>
          <ul style={{ listStyleType: "none", padding: 0, margin: 0 }}>
            {courses.map((course) => (
              <li
                key={course}
                onClick={() => setSelectedCourse(course)}
                style={{
                  padding: "0.75rem 1rem",
                  marginBottom: "0.5rem",
                  cursor: "pointer",
                  fontWeight: course === selectedCourse ? "600" : "500",
                  color: course === selectedCourse ? "#ffffff" : "#475569",
                  backgroundColor: course === selectedCourse ? "#3b82f6" : "transparent",
                  borderRadius: "10px",
                  transition: "all 0.2s ease",
                }}
              >
                {course === "english" ? "İngilizce" : course === "genel-yetenek" ? "Genel Yetenek" : "Mülakat"}
              </li>
            ))}
          </ul>
        </div>
      </aside>
    </div>
  );
}

export default App;
