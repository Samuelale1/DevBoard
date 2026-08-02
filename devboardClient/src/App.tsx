// src/App.tsx
import { useEffect, useState } from "react";
import { loginUser, fetchIssues, changeIssueStatus, setAccessToken, type Issue } from "./api/client";
import { useSignalR } from "./hooks/useSignalR";

export default function App() {
  const [token, setToken] = useState<string | null>(null);
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [projectId, setProjectId] = useState("");
  const [issues, setIssues] = useState<Issue[]>([]);
  const [error, setError] = useState("");

  const loadIssues = async () => {
    if (!projectId) return;
    try { setIssues((await fetchIssues(projectId)).items); }
    catch (e) { setError(String(e)); }
  };

  const { connected, joinProject } = useSignalR(token, loadIssues);

  useEffect(() => { if (token && projectId) joinProject(projectId); }, [token, projectId]);

  const handleLogin = async () => {
    try {
      const res = await loginUser(email, password);
      setAccessToken(res.accessToken);
      setToken(res.accessToken);
    } catch (e) { setError(String(e)); }
  };

  if (!token) {
    return (
      <div style={{ padding: 40 }}>
        <h2>DevBoard Login</h2>
        <input placeholder="email" value={email} onChange={e => setEmail(e.target.value)} /><br />
        <input placeholder="password" type="password" value={password} onChange={e => setPassword(e.target.value)} /><br />
        <button onClick={handleLogin}>Login</button>
        {error && <p style={{ color: "red" }}>{error}</p>}
      </div>
    );
  }

  return (
    <div style={{ padding: 40 }}>
      <h2>DevBoard {connected ? "🔴 Live" : "⚪ Connecting..."}</h2>
      <input placeholder="Project ID" value={projectId} onChange={e => setProjectId(e.target.value)} />
      <button onClick={loadIssues}>Load Issues</button>
      <ul>
        {issues.map(i => (
          <li key={i.id}>
            [{i.issueKey}] {i.title} — <b>{i.status}</b>
            <button onClick={async () => { await changeIssueStatus(i.id, "Todo"); loadIssues(); }}>→ Todo</button>
          </li>
        ))}
      </ul>
    </div>
  );
}