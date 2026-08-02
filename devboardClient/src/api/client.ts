
const API_URL = import.meta.env.VITE_API_URL;

let accessToken: string | null = null;
export const setAccessToken = (token: string | null) => { accessToken = token; };

async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
  const headers: HeadersInit = {
    "Content-Type": "application/json",
    ...(accessToken ? { Authorization: `Bearer ${accessToken}` } : {}),
    ...options.headers,
  };
  const res = await fetch(`${API_URL}${path}`, { ...options, headers });
  if (!res.ok) throw new Error(`${res.status}: ${await res.text()}`);
  return res.status === 204 ? (undefined as T) : res.json();
}

export interface LoginResponse { accessToken: string; refreshToken: string; expiresAt: string }
export interface Issue {
  id: string; title: string; description: string | null; status: string;
  type: string; priority: string; issueKey: string; projectId: string; assigneeId: string | null;
}
export interface PagedResult<T> { items: T[]; totalCount: number; page: number; pageSize: number }

export const registerUser = (email: string, password: string, displayName: string, workspaceId: string) =>
  request<LoginResponse>("/api/auth/register", {
    method: "POST",
    body: JSON.stringify({ email, password, displayName, workspaceId }),
  });

export const loginUser = (email: string, password: string) =>
  request<LoginResponse>("/api/auth/login", { method: "POST", body: JSON.stringify({ email, password }) });

export const fetchIssues = (projectId: string, page = 1, pageSize = 20) =>
  request<PagedResult<Issue>>(`/api/issues?projectId=${projectId}&page=${page}&pageSize=${pageSize}`);

export const createIssue = (data: { projectId: string; title: string; description?: string; type: string; priority: number }) =>
  request<Issue>("/api/issues", { method: "POST", body: JSON.stringify(data) });

export const changeIssueStatus = (id: string, newStatus: string) =>
  request<void>(`/api/issues/${id}/status`, { method: "PATCH", body: JSON.stringify({ newStatus }) });