// import axios, { AxiosInstance } from "axios";
// import Cookies from "js-cookie";

// const baseURL = process.env.NEXT_PUBLIC_API_BASE_URL || "";

// const axiosInstance: AxiosInstance = axios.create({
//   baseURL,
// });

// // Interceptor để chèn token vào header (nếu có)
// axiosInstance.interceptors.request.use(
//   (config) => {
//     if (typeof window !== "undefined") {
//       const token = Cookies.get("accessToken");
//       if (token && config.headers) {
//         config.headers.Authorization = `Bearer ${token}`;
//       }
//     }
//     return config;
//   },
//   (error) => {
//     return Promise.reject(error);
//   }
// );

// export default axiosInstance;
