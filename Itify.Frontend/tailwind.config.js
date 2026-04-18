/** @type {import('tailwindcss').Config} */
import withMT from "@material-tailwind/react/utils/withMT";

export default withMT({
    content: [
        "./index.html",
        "./src/**/*.{html,js,jsx,ts,tsx}"
    ],
    theme: {
        extend: {
            colors: {
                "app-blue": 'rgba(60, 135, 190, 1)',
                "app-dark-blue": 'rgba(36, 85, 125, 1)',
                "app-light-blue": 'rgba(0, 170, 255, 1)',
                "app-blue-start": "rgba(85, 125, 137, 1)",
                "app-blue-end": "rgba(85, 105, 137, 1)",
                "app-light-blue-start": "rgba(0, 170, 255, 1)",
                "app-light-blue-end": "rgba(0, 130, 255, 1)",
                "app-red-start": "rgba(198, 54, 121, 1)",
                "app-red-end": "rgba(118, 9, 22, 1)",
                "app-light-red-start": "rgba(255, 70, 157, 1)",
                "app-light-red-end": "rgba(168, 11, 29, 1)",
                "app-yellow-start": "rgba(224, 200, 41, 1)",
                "app-yellow-end": "rgba(151, 106, 8, 1)",
                "app-light-yellow-start": "rgba(250, 223, 46, 1)",
                "app-light-yellow-end": "rgba(236, 164, 10, 1)",
            },
            boxShadow: {
                "card": "0 3px 12px rgba(0, 0, 0, 0.2)"
            }
        }
    },
    plugins: [],
});

