# Parcellation UI

A Next.js 14 application with TypeScript, Tailwind CSS, and react-icons. This UI is designed to be integrated as a webview in native applications.

## Features

- Next.js 14 with App Router
- TypeScript for type safety
- Tailwind CSS for styling
- react-icons for beautiful icons
- Responsive design works on all device sizes
- Dark mode support

## Getting Started

### Prerequisites

- Node.js 18.17 or later

### Installation

1. Clone the repository:

```bash
git clone <repository-url>
cd parcellation-ui
```

2. Install dependencies:

```bash
npm install
```

3. Run the development server:

```bash
npm run dev
```

4. Open [http://localhost:3000](http://localhost:3000) in your browser to see the result.

## Usage in a Native Application Webview

When integrating this UI into a native application webview:

1. Build the project:

```bash
npm run build
```

2. The build output will be generated in the `.next` directory.

3. To serve the built UI:

```bash
npm run start
```

4. In your native application, load the webview from the appropriate URL.

## Customization

- Modify `src/components` to create or update UI components
- Update styles in component files using Tailwind CSS classes
- Add new pages in `src/app` directory

## Project Structure

```
parcellation-ui/
├── src/
│   ├── app/
│   │   ├── page.tsx      # Main page
│   │   ├── layout.tsx    # Root layout
│   │   └── globals.css   # Global styles
│   └── components/
│       ├── Header.tsx    # Header component with react-icons
│       └── Dashboard.tsx # Dashboard component
├── public/
│   └── ...               # Static assets
├── package.json
└── ...                   # Configuration files
```

## License

This project is licensed under the MIT License.
