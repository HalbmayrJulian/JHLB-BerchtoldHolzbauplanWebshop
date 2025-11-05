# Admin-Zugangsdaten

## Standard Admin-Benutzer

Beim ersten Start der Anwendung wird automatisch ein Admin-Benutzer erstellt:

- **E-Mail:** admin@berchtold.com
- **Passwort:** Admin123!
- **Rolle:** Admin

## Funktionsweise

1. Melden Sie sich mit den oben genannten Anmeldedaten an
2. Im Dropdown-Menü (oben rechts bei Ihrem Namen) erscheint ein neuer Eintrag "Admin Panel"
3. Klicken Sie auf "Admin Panel", um zur Admin-Seite zu gelangen
4. Die Admin-Seite ist nur für Benutzer mit der Rolle "Admin" zugänglich

## Hinweise

- Normale Benutzer, die sich registrieren, erhalten automatisch die Rolle "User"
- Der "Admin Panel" Link ist nur für Benutzer mit Admin-Rolle sichtbar
- Die Admin-Seite ist durch `[Authorize(Roles = "Admin")]` geschützt
