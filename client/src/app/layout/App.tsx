import { Container, createTheme, CssBaseline, ThemeProvider } from "@mui/material";
import '../../App.css';
import {useState} from "react";
import Header from "./Header";
import Catalog from "../features/Catalog";

export default function App() {
  const [darkMode, setDarkMode] = useState(false);
  const paletteType  = darkMode ? 'dark' : 'light'
  const theme = createTheme({
    palette: {
      mode: paletteType,
      background: {
        default: paletteType === 'light' ? '#eaeaea' : '#121212'
      }
    }
  })

  function handleThemeChange() {
    setDarkMode(!darkMode);
  }
  return (
      <ThemeProvider theme={theme}>
        <CssBaseline />
        <Header darkMode={darkMode} handleThemeChange={handleThemeChange} />
        <Container>
          <Catalog/>
        </Container>
      </ThemeProvider>
  );
}
