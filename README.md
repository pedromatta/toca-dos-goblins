# Toca dos Goblins

Este é um projeto de RPG desenvolvido em Unity, disponível tanto para compilação local quanto para execução direta via itch.io.

## Sobre

Um RPG 2D com sistema de combate, missões, inventário, progressão de personagem e exploração de áreas, totalmente implementado em C# utilizando Unity.

## Como Jogar

### 1. Jogar Online (Recomendado)

O jogo está publicado no itch.io. Basta acessar o link abaixo e jogar diretamente no navegador, sem necessidade de instalação:

[**Jogar no itch.io**](https://pedromatta.itch.io/toca-dos-goblins-web)  

### 2. Compilar e Executar Localmente

#### Pré-requisitos

- [Unity Hub](https://unity.com/download) instalado
- Unity Editor versão **6.1** ou superior
- Git (opcional, para clonar o repositório)

#### Passos para Compilar

1. **Clone o repositório:**

```bash
git clone https://github.com/pedromatta/toca-dos-goblins.git 
```

3. **Abra o projeto no Unity:**
- Abra o Unity Hub.
- Clique em "Add" e selecione a pasta do projeto clonado.
- Aguarde o Unity importar todos os assets e dependências.

3. **Compile e execute:**
- Com o projeto aberto, clique em `File > Build and Run` para compilar e executar.
- Ou clique em `Play` na barra superior para rodar no editor.

#### Build para WebGL

Se desejar gerar sua própria build WebGL:

- No Unity, vá em `File > Build Settings`.
- Selecione `WebGL` e clique em `Build`.
- O conteúdo gerado pode ser hospedado em qualquer servidor web ou publicado no itch.io.

## Estrutura do Projeto

- `Assets/`: Scripts, cenas, prefabs, sprites e assets do jogo.
- `ProjectSettings/`: Configurações do projeto Unity.
- `Packages/`: Dependências do projeto.
