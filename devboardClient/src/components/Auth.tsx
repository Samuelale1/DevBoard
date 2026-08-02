// src/components/Auth.tsx
import { useState } from "react";
import { loginUser, registerUser, setAccessToken } from "../api/client";

interface AuthProps {
  onAuthenticated: (token: string) => void;
}

export default function Auth({ onAuthenticated }: AuthProps) {
  const [darkMode, setDarkMode] = useState(false);
  const [isLoginMode, setIsLoginMode] = useState(true);
  const [formData, setFormData] = useState({
    displayName: "",
    email: "",
    workspaceId: "",
    password: "",
  });
  const [error, setError] = useState("");
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(false);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({ ...formData, [e.target.name]: e.target.value });
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError("");
    setMessage("");
    setLoading(true);

    try {
      const res = isLoginMode
        ? await loginUser(formData.email, formData.password)
        : await registerUser(formData.email, formData.password, formData.displayName, formData.workspaceId);

      setAccessToken(res.accessToken);
      setMessage(isLoginMode ? "Welcome back!" : "Account created!");
      onAuthenticated(res.accessToken);
    } catch (err) {
      setError(String(err).includes("401") ? "Invalid email or password." : "Something went wrong. Please try again.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className={darkMode ? "dark" : ""}>
      <div className="flex w-full min-h-screen bg-white dark:bg-slate-900 font-sans transition-colors duration-500 overflow-hidden relative">

        <div className="absolute top-6 right-6 z-50">
          <button
            onClick={() => setDarkMode(!darkMode)}
            className="p-3 rounded-2xl bg-gray-100 dark:bg-white/5 border border-gray-200 dark:border-white/10 text-gray-600 dark:text-gray-400 hover:text-amber-500 dark:hover:text-amber-400 transition-all duration-300 shadow-sm"
          >
            {darkMode ? "☀️" : "🌙"}
          </button>
        </div>

        <div className="absolute inset-0 z-0 pointer-events-none">
          <div className="absolute -top-24 -left-24 w-96 h-96 bg-amber-500/10 dark:bg-amber-500/20 rounded-full blur-3xl"></div>
          <div className="absolute top-1/2 -right-24 w-80 h-80 bg-orange-600/10 dark:bg-orange-600/20 rounded-full blur-3xl"></div>
        </div>

        <div className="w-full flex items-center justify-center relative z-10 px-4 py-12">
          <div className="w-full max-w-md bg-white/90 dark:bg-white/5 backdrop-blur-2xl border border-gray-100 dark:border-white/10 p-8 rounded-[2.5rem] shadow-[0_20px_50px_rgba(0,0,0,0.08)] dark:shadow-none transition-all duration-300 relative z-10">

            <div className="text-center mb-8">
              <h1 className="text-3xl font-extrabold text-slate-900 dark:text-white tracking-tight">
                DevBoard<span className="text-amber-500">.</span>
              </h1>
              <p className="text-gray-500 dark:text-gray-400 mt-2 text-sm">
                {isLoginMode ? "Welcome back to your board." : "Create a workspace account."}
              </p>
            </div>

            <div className="relative flex h-14 bg-gray-100 dark:bg-slate-950/50 rounded-2xl p-1 mb-8 overflow-hidden">
              <div
                className={`absolute top-1 bottom-1 w-[calc(50%-4px)] bg-gradient-to-r from-amber-500 to-orange-600 rounded-xl transition-all duration-300 ease-out shadow-lg shadow-orange-500/20 ${isLoginMode ? "left-1" : "left-[calc(50%+4px)]"}`}
              ></div>
              <button
                onClick={() => setIsLoginMode(true)}
                className={`flex-1 relative z-10 text-sm font-black transition-colors duration-200 ${isLoginMode ? "text-white" : "text-gray-500 dark:text-gray-400 hover:text-slate-900 dark:hover:text-white"}`}
              >
                Login
              </button>
              <button
                onClick={() => setIsLoginMode(false)}
                className={`flex-1 relative z-10 text-sm font-black transition-colors duration-200 ${!isLoginMode ? "text-white" : "text-gray-500 dark:text-gray-400 hover:text-slate-900 dark:hover:text-white"}`}
              >
                Sign Up
              </button>
            </div>

            <form onSubmit={handleSubmit} className="space-y-4">
              {!isLoginMode && (
                <div className="space-y-1">
                  <label className="text-[10px] font-black text-gray-400 dark:text-gray-500 uppercase tracking-widest ml-1">Display Name</label>
                  <input
                    type="text" name="displayName" placeholder="Samuel"
                    value={formData.displayName} onChange={handleChange} required
                    className="w-full p-4 rounded-xl bg-gray-50 dark:bg-slate-800/50 border border-gray-100 dark:border-slate-700 text-slate-800 dark:text-white placeholder-gray-300 dark:placeholder-gray-500 focus:outline-none focus:border-amber-500 focus:ring-4 focus:ring-amber-500/10 transition-all font-medium"
                  />
                </div>
              )}

              <div className="space-y-1">
                <label className="text-[10px] font-black text-gray-400 dark:text-gray-500 uppercase tracking-widest ml-1">Email Address</label>
                <input
                  type="email" name="email" placeholder="you@example.com"
                  value={formData.email} onChange={handleChange} required
                  className="w-full p-4 rounded-xl bg-gray-50 dark:bg-slate-800/50 border border-gray-100 dark:border-slate-700 text-slate-800 dark:text-white placeholder-gray-300 dark:placeholder-gray-500 focus:outline-none focus:border-amber-500 focus:ring-4 focus:ring-amber-500/10 transition-all font-medium"
                />
              </div>

              {!isLoginMode && (
                <div className="space-y-1">
                  <label className="text-[10px] font-black text-gray-400 dark:text-gray-500 uppercase tracking-widest ml-1">Workspace ID</label>
                  <input
                    type="text" name="workspaceId" placeholder="3fa85f64-5717-4562-b3fc-2c963f66afa6"
                    value={formData.workspaceId} onChange={handleChange} required
                    className="w-full p-4 rounded-xl bg-gray-50 dark:bg-slate-800/50 border border-gray-100 dark:border-slate-700 text-slate-800 dark:text-white placeholder-gray-300 dark:placeholder-gray-500 focus:outline-none focus:border-amber-500 focus:ring-4 focus:ring-amber-500/10 transition-all font-medium"
                  />
                </div>
              )}

              <div className="space-y-1">
                <label className="text-[10px] font-black text-gray-400 dark:text-gray-500 uppercase tracking-widest ml-1">Password</label>
                <input
                  type="password" name="password" placeholder="••••••••"
                  value={formData.password} onChange={handleChange} required
                  className="w-full p-4 rounded-xl bg-gray-50 dark:bg-slate-800/50 border border-gray-100 dark:border-slate-700 text-slate-800 dark:text-white placeholder-gray-300 dark:placeholder-gray-500 focus:outline-none focus:border-amber-500 focus:ring-4 focus:ring-amber-500/10 transition-all font-medium"
                />
              </div>

              <button
                type="submit" disabled={loading}
                className="w-full py-4 mt-6 bg-gradient-to-r from-amber-500 to-orange-600 text-white font-bold rounded-xl shadow-lg shadow-orange-500/20 hover:shadow-orange-500/40 hover:-translate-y-0.5 transition-all active:scale-95 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                {loading ? "Please wait..." : isLoginMode ? "Login" : "Create Account"}
              </button>

              {error && (
                <div className="p-4 rounded-xl bg-red-500/10 border border-red-500/20 text-red-600 dark:text-red-500 text-sm text-center font-bold">
                  {error}
                </div>
              )}
              {message && (
                <div className="p-4 rounded-xl bg-emerald-500/10 border border-emerald-500/20 text-emerald-600 dark:text-emerald-500 text-sm text-center font-bold">
                  {message}
                </div>
              )}
            </form>
          </div>
        </div>
      </div>
    </div>
  );
}