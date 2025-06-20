# Toca dos Goblins

Este é um projeto de RPG desenvolvido em Unity, disponível tanto para compilação local quanto para execução direta via itch.io.

## Sobre

Um RPG 2D com sistema de combate, missões, inventário, progressão de personagem e exploração de áreas, totalmente implementado em C# utilizando Unity.

## Código

O código do jogo está disponível em Assets/Scripts. Nesse diretório estão todos os modelos de classes, controladores, managers e lógica.

## Controles

- **Navegação na tela inicial** - Mouse.
- **Navegação na criação de Personagens** - Setas para navegar, enter para confirmar decisão.
- **Navegação no Mundo** - W,A,S,D para mover o personagem, Mouse para interagir com a UI, E para interagir com NPCs, Inimigos e para mudar de Área

## Como Jogar

### 1. Jogar Online (Recomendado)

O jogo está publicado no itch.io. Basta acessar o link abaixo e jogar diretamente no navegador, sem necessidade de instalação:

[**Jogar no itch.io**](https://pedromatta.itch.io/toca-dos-goblins-web)  

### 2. Baixar o jogo já compilado

O jogo está publicado no itch.io. Basta acessar o link abaixo e instalar diretamente a build do jogo:

[**Jogar no itch.io**](https://pedromatta.itch.io/toca-dos-goblins)  


### 3. Compilar e Executar Localmente

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
