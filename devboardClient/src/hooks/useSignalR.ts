// src/hooks/useSignalR.ts
import { useEffect, useRef, useState } from "react";
import * as signalR from "@microsoft/signalr";

export function useSignalR(token: string | null, onIssueUpdated: () => void) {
  const [connected, setConnected] = useState(false);
  const connectionRef = useRef<signalR.HubConnection | null>(null);

  useEffect(() => {
    if (!token) return;

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(`${import.meta.env.VITE_API_URL}/hubs/board`, { accessTokenFactory: () => token })
      .withAutomaticReconnect()
      .build();

    connection.on("IssueUpdated", onIssueUpdated);
    connection.start().then(() => setConnected(true)).catch(console.error);
    connectionRef.current = connection;

    return () => { connection.stop(); };
  }, [token]);

  const joinProject = (projectId: string) => connectionRef.current?.invoke("JoinProject", projectId);

  return { connected, joinProject };
}