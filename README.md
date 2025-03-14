
# Accesibility Analyzer

Accessibility Analyzer is an AI-powered tool that evaluates websites for WCAG compliance using multiple specialized agents to analyze both HTML code and visual elements. The service leverages Microsoft Semantic Kernel and Playwright to capture website content, then processes it through specialized accessibility analyzers that focus on different aspects like alt text, keyboard navigation, semantic structure, color contrast, and more. Results are aggregated into a structured JSON format that details specific accessibility issues found on the analyzed website, helping developers meet accessibility standards required by regulations like the European Accessibility Act.

## Installation

* Clone the repository 
* copy `.env.example` file to `.env`
* adjust the values in `.env`

## Running the Api

The api can be run with dotnet cli or visual studio (code)

### Running the local OmniParser api server
After [installing OmniParser](https://github.com/microsoft/OmniParser/blob/master/README.md#install), you can run the server with the following command:

```bash
cd omnitool/omniparserserver
python omniparserserver.py
```

Make sure it runs on `http://localhost:8000/`.

