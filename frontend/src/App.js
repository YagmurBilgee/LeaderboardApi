import { useState, useEffect, useCallback } from "react";
import "./App.css";

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
    <div className="container">
      <div className="main-content">
        <div className="card">
          <h1>🏆 Leaderboard</h1>
          <p>Performans Takip Sistemi</p>
        </div>

        {data.length > 0 && (
          <div className="card first-card">
            <div>👑</div>
            <div>Birinci</div>
            <div>{data[0].username}</div>
            <div>{data[0].score} puan</div>
          </div>
        )}

        <div className="card">
          <h2>Skor Gönder</h2>
          <form onSubmit={handleSubmit} style={{ display: "flex", gap: "1rem", flexWrap: "wrap" }}>
            <input
              type="text"
              placeholder="Kullanıcı adı"
              value={newUsername}
              onChange={(e) => setNewUsername(e.target.value)}
              className="form-input"
            />
            <input
              type="number"
              placeholder="Skor"
              value={newScore}
              onChange={(e) => setNewScore(e.target.value)}
              className="form-input"
              style={{ width: "100px" }}
            />
            <button type="submit" className="form-button">Gönder</button>
          </form>
        </div>

        <div className="card">
          <input
            type="text"
            placeholder="🔍 Kullanıcı adı ara..."
            value={searchTerm}
            onChange={(e) => setSearchTerm(e.target.value)}
            className="search-box"
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

      <aside className="sidebar">
        <div className="card">
          <h3>📚 Kategori</h3>
          <ul style={{ listStyleType: "none", padding: 0, margin: 0 }}>
            {courses.map((course) => (
              <li
                key={course}
                onClick={() => setSelectedCourse(course)}
                className={`category-item ${course === selectedCourse ? "active" : ""}`}
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