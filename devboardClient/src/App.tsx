
import { useEffect, useState } from "react";
import { fetchIssues, changeIssueStatus, type Issue } from "./api/client";
import { useSignalR } from "./hooks/useSignalR";
import Auth from "./components/Auth";

export default function App() {
  const [token, setToken] = useState<string | null>(null);
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

  if (!token) {
    return <Auth onAuthenticated={setToken} />;
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
      {error && <p style={{ color: "red" }}>{error}</p>}
    </div>
  );
}