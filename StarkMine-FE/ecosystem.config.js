module.exports = {
  apps: [
    {
      name: "starkmine-game",
      script: "npm",
      args: "start",
      cwd: "./",
      watch: true,
      ignore_watch: ["node_modules", ".next"],
    },
  ],
};
