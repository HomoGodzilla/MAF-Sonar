let map;
let miniMap;
let markers = [];
let proximoId = 4;
let marcadorTemporario = null;
let listenerMapa = null;
let infoWindow = null;

let arvores = [
  { 
    id: 1, 
    name: "Mangueira", 
    tipo: "exotica", 
    location: { latitude: -21.13, longitude: -44.23 }, 
    imagem: null,
    descricao: "Árvore frutífera tropical, produz manga, muito comum no Brasil."
  },
  { 
    id: 2, 
    name: "Goiabeira", 
    tipo: "nativa", 
    location: { latitude: -21.129, longitude: -44.229 }, 
    imagem: null,
    descricao: "Árvore nativa da América Tropical, produz goiaba, rica em vitamina C."
  },
  { 
    id: 3, 
    name: "Laranjeira", 
    tipo: "exotica", 
    location: { latitude: -21.129, longitude: -44.227 }, 
    imagem: null,
    descricao: "Árvore cítrica originária da Ásia, produz laranjas, fonte de vitamina C."
  },
];

function initMap() {
  map = new google.maps.Map(document.getElementById("map"), {
    center: { lat: -21.1298, lng: -44.2377 },
    zoom: 13,
    mapTypeId: "roadmap"
  });

  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(
      pos => {
        const user = { lat: pos.coords.latitude, lng: pos.coords.longitude };
        map.setCenter(user);
        map.setZoom(15);

        new google.maps.Marker({
          position: user,
          map,
          title: "Você está aqui!",
          icon: "https://maps.google.com/mapfiles/ms/icons/blue-dot.png"
        });

        atualizarMarcadores();
      },
      () => atualizarMarcadores()
    );
  } else {
    atualizarMarcadores();
  }
  carregarArvoresDoServidor();
}

function atualizarMarcadores(lista = arvores) {
  markers.forEach(m => {
    if (m.infoWindow) {
      m.infoWindow.close();
    }
    m.setMap(null);
  });
  markers = [];
  
  if (!infoWindow) {
    infoWindow = new google.maps.InfoWindow();
  }

  lista.forEach(a => {
    const marker = new google.maps.Marker({
      position: { lat: a.location.latitude, lng: a.location.longitude },
      map,
      title: a.name,
      icon:
        a.tipo === "nativa"
          ? "https://maps.google.com/mapfiles/ms/icons/green-dot.png"
          : "https://maps.google.com/mapfiles/ms/icons/red-dot.png"
    });

    marker.addListener('click', () => {
      mostrarDetalhesArvore(a, marker);
    });

    markers.push(marker);
  });
}

function mostrarDetalhesArvore(arvore, marker) {
  if (!infoWindow) {
    infoWindow = new google.maps.InfoWindow();
  }
  
  infoWindow.close();
  
  let conteudo = `
    <div style="max-width: 320px; font-family: Arial, sans-serif; background-color: #f5f5f5; color: #333; padding: 20px; border-radius: 10px; box-shadow: 0 4px 12px rgba(0,0,0,0.15);">
      <h3 style="margin: 0 0 15px 0; color: #2c3e50; border-bottom: 2px solid #95a5a6; padding-bottom: 8px; font-size: 18px;">${arvore.name}</h3>
      
      <div style="margin-bottom: 12px; background-color: #e0e0e0; padding: 10px; border-radius: 6px; border-left: 4px solid #7f8c8d;">
        <strong style="color: #2c3e50;">Tipo:</strong> 
        <span style="color: ${arvore.tipo === "nativa" ? '#27ae60' : '#e74c3c'}; font-weight: bold; margin-left: 5px;">
          ${arvore.tipo === "nativa" ? "🌿 Nativa" : "🌍 Exótica"}
        </span>
      </div>
  `;
  
  if (arvore.imagem) {
    conteudo += `
      <div style="margin-bottom: 12px; text-align: center; background-color: #ecf0f1; padding: 10px; border-radius: 6px;">
        <img src="${arvore.imagem}" 
             alt="${arvore.name}" 
             style="width: 100%; max-height: 180px; object-fit: cover; border-radius: 6px; border: 3px solid #bdc3c7;">
      </div>
    `;
  }
  
  conteudo += `
    <div style="margin-bottom: 12px; background-color: #e8e8e8; padding: 12px; border-radius: 6px; border-left: 4px solid #3498db;">
      <strong style="color: #2c3e50; display: block; margin-bottom: 5px;">📝 Descrição:</strong>
      <div style="color: #34495e; line-height: 1.5; font-size: 14px; background-color: #f8f9fa; padding: 10px; border-radius: 4px;">
        ${arvore.descricao || "Sem descrição cadastrada."}
      </div>
    </div>
  `;
  
  conteudo += `
    <div style="margin-bottom: 15px; background-color: #d5dbdb; padding: 10px; border-radius: 6px; border-left: 4px solid #95a5a6;">
      <strong style="color: #2c3e50; display: block; margin-bottom: 5px;">📍 Coordenadas:</strong>
      <div style="font-family: 'Courier New', monospace; color: #2c3e50; font-size: 13px; background-color: #ecf0f1; padding: 8px; border-radius: 4px;">
        <div>Lat: <strong>${arvore.location.latitude.toFixed(6)}</strong></div>
        <div>Lng: <strong>${arvore.location.longitude.toFixed(6)}</strong></div>
      </div>
    </div>
    
    <div style="display: flex; gap: 10px; margin-top: 15px;">
      <button onclick="window.editarArvore(${arvore.id})" 
              style="padding: 10px 15px; background: linear-gradient(45deg, #3498db, #2980b9); color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; flex: 1; font-size: 14px; transition: all 0.3s;">
        ✏️ Editar
      </button>
      <button onclick="window.fecharInfoWindow()" 
              style="padding: 10px 15px; background: linear-gradient(45deg, #95a5a6, #7f8c8d); color: white; border: none; border-radius: 6px; cursor: pointer; font-weight: bold; flex: 1; font-size: 14px; transition: all 0.3s;">
        ✕ Fechar
      </button>
    </div>
  `;
  
  conteudo += `</div>`;
  
  infoWindow.setContent(conteudo);
  infoWindow.open(map, marker);
  
  google.maps.event.addListenerOnce(infoWindow, 'domready', () => {
    const fecharBtn = document.querySelector('[onclick="window.fecharInfoWindow()"]');
    if (fecharBtn) {
      fecharBtn.addEventListener('click', fecharInfoWindow);
      fecharBtn.addEventListener('mouseenter', function() {
        this.style.background = "linear-gradient(45deg, #7f8c8d, #95a5a6)";
        this.style.transform = "translateY(-2px)";
      });
      fecharBtn.addEventListener('mouseleave', function() {
        this.style.background = "linear-gradient(45deg, #95a5a6, #7f8c8d)";
        this.style.transform = "translateY(0)";
      });
    }
    
    const editarBtn = document.querySelector('[onclick="window.editarArvore(' + arvore.id + ')"]');
    if (editarBtn) {
      editarBtn.addEventListener('click', function() {
        editarArvore(arvore.id);
      });
      editarBtn.addEventListener('mouseenter', function() {
        this.style.background = "linear-gradient(45deg, #2980b9, #3498db)";
        this.style.transform = "translateY(-2px)";
      });
      editarBtn.addEventListener('mouseleave', function() {
        this.style.background = "linear-gradient(45deg, #3498db, #2980b9)";
        this.style.transform = "translateY(0)";
      });
    }
  });
}

function fecharInfoWindow() {
  if (infoWindow) {
    infoWindow.close();
  }
}

// NOVA FUNÇÃO: Logout
function fazerLogout() {
  if (confirm("Tem certeza que deseja sair?")) {
    // Fechar a sidebar
    document.getElementById("sidebar").classList.remove("active");
    
    // Limpar dados temporários se necessário
    fecharInfoWindow();
    fecharModal();
    
    // Redirecionar para index.html
    window.location.href = "index.html";
  }
}

function mostrarTabelaArvores() {
  document.getElementById("map").style.display = "none";
  document.getElementById("tabelaArvores").style.display = "flex";
  renderizarTabela();
}

function mostrarMapa() {
  document.getElementById("tabelaArvores").style.display = "none";
  document.getElementById("map").style.display = "block";
  fecharModal();
}

function renderizarTabela(lista = arvores) {
  const corpo = document.getElementById("listaArvores");
  corpo.innerHTML = "";

  lista.forEach(a => {
    const temImagem = a.imagem ? "✅" : "-";
    const descricaoResumida = a.descricao ? 
      (a.descricao.length > 30 ? a.descricao.substring(0, 30) + '...' : a.descricao) : 
      "-";
    
    corpo.innerHTML += `
      <tr>
        <td>${a.id}</td>
        <td class="especie" onclick="mostrarMiniMapa(${a.location.latitude}, ${a.location.longitude}, '${a.name}')">
          ${a.name}
        </td>
        <td>${a.location.latitude.toFixed(4)}</td>
        <td>${a.location.longitude.toFixed(4)}</td>
        <td data-tipo="${a.tipo}">
          ${a.tipo === "nativa" ? "Nativa" : "Exótica"}
        </td>
        <td title="${a.descricao || ''}" style="max-width: 200px; overflow: hidden; text-overflow: ellipsis; white-space: nowrap;">
          ${descricaoResumida}
        </td>
        <td>${temImagem}</td>
        <td>
          <button onclick="editarArvore(${a.id})">✏️ Editar</button>
          <button onclick="excluirArvore(${a.id})">🗑️ Excluir</button>
        </td>
      </tr>
    `;
  });

  atualizarMarcadores(lista);
}

function filtrarArvores() {
  const texto = document.getElementById("pesquisaArvore").value.toLowerCase();
  const tipoFiltro = document.getElementById("filtroTipo").value;

  const filtradas = arvores.filter(a => {
    const matchNome = a.name.toLowerCase().includes(texto);
    const matchTipo = tipoFiltro === "" || a.tipo === tipoFiltro;
    return matchNome && matchTipo;
  });

  renderizarTabela(filtradas);
}

function mostrarMiniMapa(lat, lng, especie) {
  const box = document.getElementById("miniMapa");
  box.style.display = "block";
  box.innerHTML = "";

  miniMap = new google.maps.Map(box, {
    center: { lat, lng },
    zoom: 17
  });

  new google.maps.Marker({
    position: { lat, lng },
    map: miniMap,
    title: especie,
    icon: "https://maps.google.com/mapfiles/ms/icons/orange-dot.png"
  });
}

function editarArvore(id) {
  const a = arvores.find(x => x.id === id);
  if (!a) return alert("Erro ao editar!");

  const novaEspecie = prompt("Nova espécie:", a.name);
  if (!novaEspecie) return;

  const novoTipo = prompt("Tipo (nativa / exotica):", a.tipo);
  if (!novoTipo) return;
  
  const novaDescricao = prompt("Descrição da árvore:", a.descricao || "");

  a.name = novaEspecie;
  a.tipo = novoTipo === "nativa" ? "nativa" : "exotica";
  a.descricao = novaDescricao;

  fecharInfoWindow();
  renderizarTabela();
  atualizarMarcadores();
}

function excluirArvore(id) {
  if (!confirm("Tem certeza que deseja excluir esta árvore?")) return;

  arvores = arvores.filter(a => a.id !== id);
  fecharInfoWindow();
  renderizarTabela();
  atualizarMarcadores();
  alert("Árvore excluída com sucesso!");
}

function toggleSidebar() {
  document.getElementById("sidebar").classList.toggle("active");
}

function mostrarModalAdicionar() {
  document.getElementById('modalAdicionar').style.display = 'block';
  adicionarListenerMapa();
}

function fecharModal() {
  document.getElementById('modalAdicionar').style.display = 'none';
  removerListenerMapa();
  limparMarcadorTemporario();
  document.getElementById('formArvore').reset();
}

function capturarLocalizacao() {
  if (navigator.geolocation) {
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        document.getElementById('latitude').value = pos.coords.latitude.toFixed(6);
        document.getElementById('longitude').value = pos.coords.longitude.toFixed(6);
        
        const posicao = {
          lat: pos.coords.latitude,
          lng: pos.coords.longitude
        };
        
        map.setCenter(posicao);
        map.setZoom(17);
        adicionarMarcadorTemporario(posicao);
      },
      (erro) => {
        alert('Erro ao capturar localização: ' + erro.message);
      }
    );
  } else {
    alert('Geolocalização não suportada pelo navegador');
  }
}

function selecionarNoMapa() {
  alert('Agora clique no mapa para selecionar a localização da árvore');
  adicionarListenerMapa();
}

function adicionarListenerMapa() {
  removerListenerMapa();
  
  listenerMapa = map.addListener('click', (evento) => {
    const lat = evento.latLng.lat();
    const lng = evento.latLng.lng();
    
    document.getElementById('latitude').value = lat.toFixed(6);
    document.getElementById('longitude').value = lng.toFixed(6);
    
    adicionarMarcadorTemporario(evento.latLng);
  });
}

function removerListenerMapa() {
  if (listenerMapa) {
    google.maps.event.removeListener(listenerMapa);
    listenerMapa = null;
  }
}

function adicionarMarcadorTemporario(localizacao) {
  limparMarcadorTemporario();
  
  marcadorTemporario = new google.maps.Marker({
    position: localizacao,
    map: map,
    icon: 'https://maps.google.com/mapfiles/ms/icons/yellow-dot.png',
    animation: google.maps.Animation.BOUNCE
  });
}

function limparMarcadorTemporario() {
  if (marcadorTemporario) {
    marcadorTemporario.setMap(null);
    marcadorTemporario = null;
  }
}

document.getElementById('formArvore').addEventListener('submit', function(e) {
  e.preventDefault();
  
  const especie = document.getElementById('especie').value.trim();
  const tipo = document.getElementById('tipo').value;
  const latitude = parseFloat(document.getElementById('latitude').value);
  const longitude = parseFloat(document.getElementById('longitude').value);
  const descricao = document.getElementById('descricao').value.trim();
  const imagemInput = document.getElementById('imagem');
  
  if (!especie) {
    alert('Por favor, informe a espécie da árvore');
    return;
  }
  
  if (!tipo) {
    alert('Por favor, selecione o tipo da árvore');
    return;
  }
  
  if (isNaN(latitude) || isNaN(longitude)) {
    alert('Por favor, informe coordenadas válidas');
    return;
  }
  
  if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180) {
    alert('Por favor, insira coordenadas válidas');
    return;
  }

  let imagemUrl = null;
  if (imagemInput.files[0]) {
    const arquivo = imagemInput.files[0];
    imagemUrl = URL.createObjectURL(arquivo);
  }

  const novaArvore = {
    id: proximoId++,
    name: especie,
    tipo: tipo,
    location: { latitude, longitude },
    imagem: imagemUrl,
    descricao: descricao || `Árvore ${especie} do tipo ${tipo}.`
  };

  arvores.push(novaArvore);
  
  atualizarMarcadores();
  renderizarTabela();
  fecharModal();
  
  alert('Árvore cadastrada com sucesso!');
});

window.onclick = function(event) {
  const modal = document.getElementById('modalAdicionar');
  if (event.target == modal) {
    fecharModal();
  }
}

document.addEventListener('DOMContentLoaded', function() {
  const imagemInput = document.getElementById('imagem');
  imagemInput.addEventListener('change', function(e) {
    const file = e.target.files[0];
    if (file) {
      if (!file.type.match('image.*')) {
        alert('Por favor, selecione apenas arquivos de imagem');
        this.value = '';
        return;
      }
    }
  });
});

async function carregarArvoresDoServidor() {
  const API_BASE_URL = 'https://maf-api-service-179671765640.us-central1.run.app';

  try {
    const response = await fetch(`${API_BASE_URL}/maf/t/get_all_trees`);
    
    if (!response.ok) {
      throw new Error('Falha ao buscar árvores');
    }

    const listaDoServidor = await response.json();

    // Mapear os dados da API para o formato que seu script.js usa
    // TreeDtoRes (API) -> formato local (arvores)
    arvores = listaDoServidor.map((tree, index) => {
      return {
        id: index + 100, // ID fictício para o front não se perder
        name: tree.name, // A API retorna camelCase por padrão no JSON (name, fruit, etc)
        // Usamos 'fruit' como 'tipo' (nativa/exotica) conforme conversado no cadastro
        tipo: tree.fruit ? tree.fruit.toLowerCase() : "exotica", 
        location: { 
          latitude: tree.latitude, 
          longitude: tree.longitude 
        },
        // Se a UrlBinPhoto for vazia, deixa null, senão usa ela
        imagem: tree.urlBinPhoto || null, 
        descricao: tree.description
      };
    });

    console.log("Árvores carregadas:", arvores);
    
    // Atualiza o mapa e a tabela com os novos dados
    atualizarMarcadores();
    if(document.getElementById("tabelaArvores").style.display !== "none"){
        renderizarTabela();
    }

  } catch (error) {
    console.error('Erro ao carregar árvores:', error);
    alert('Não foi possível carregar as árvores do servidor.');
  }
}